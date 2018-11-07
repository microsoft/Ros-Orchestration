#!/usr/bin/env python

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import rospy
import actionlib
import os
import json
import threading
import sys
from datetime import datetime 
from std_msgs.msg import String
from orchestrator_msgs.srv import RobotInfo, RobotInfoResponse
from robot_status import RobotStatus
from job_status import JobStatus

from fleet_manager_client import FleetManagerClient
from jobs_handler import JobsHandler
from move_base_client import MoveBaseClient

class RobotController:

    def __init__(self, robot_id, fleet_manager_url, fleet_manager_version):
        rospy.init_node('robot_controller', anonymous=True, log_level=rospy.DEBUG)

        self.robot_id = robot_id
        self.current_job = None
        self.state_lock = threading.Lock()
        self.job_lock = threading.Lock()

        self.robot_status = RobotStatus.Onboarding
        self.onboard_robot(fleet_manager_url, fleet_manager_version)
        self.robot_status = RobotStatus.Idle

        self.jobs_handler = JobsHandler(self)

        # set up robot information server
        self.set_up_server()

    def onboard_robot(self, fleet_manager_url, fleet_manager_version):
        fleet_manager_client = FleetManagerClient(fleet_manager_url, fleet_manager_version)
        iot_connection_string = fleet_manager_client.get_robot_config(self.robot_id)
        self.launch_iot_relay(iot_connection_string)

    def launch_iot_relay(self, iot_connection_string):
        print("Starting iot relay with connection string:%s", iot_connection_string)
        launch_cmd = 'roslaunch orchestrated_robot relay.launch connection_string:=' + iot_connection_string + " &"
        os.system(launch_cmd)

    def set_up_server(self):
        rospy.Service('get_robot_info', RobotInfo, self.get_robot_info)
        rospy.spin()

    def update_robot_state(self, status, job):

        self.state_lock.acquire()
        rospy.loginfo("Updating robot state to: %s", status.value)
        try:
            self.robot_status = status
            self.current_job = job
        finally:
            self.state_lock.release()

    def get_robot_info(self, req):

        info = RobotInfoResponse()
        info.RobotId = self.robot_id
        info.Status = self.robot_status.value
        info.OrderId = self.current_job.OrderId if self.current_job != None else ""
        
        return info

    def handle_job_new(self, job):
        
        try:
            rospy.loginfo("Got Job: " + str(job))

            self.job_lock.acquire()
            try:

                # Decline job if robot is busy
                if (self.robot_status == RobotStatus.Busy):
                    self.handle_job_fail(job)
                    return 
                
                job.Status = JobStatus.InProgress.value
                self.update_robot_state(RobotStatus.Busy, job)
                
            finally:
                self.job_lock.release()
            
            self.execute_job(job)

        except Exception as e:
            rospy.logerr("Something went wrong: " + str(e))
            self.handle_job_fail(job)
            self.set_robot_to_failed()

        # Flush stdout
        sys.stdout.flush()

    def execute_job(self, job): 

        move_base_client = MoveBaseClient(job.id)
    
        start_pose = move_base_client.create_pose(job.StartPosition.X, job.StartPosition.Y)
        end_pose = move_base_client.create_pose(job.EndPosition.X, job.EndPosition.Y)

        result = move_base_client.move(start_pose)

        if not result:
            rospy.loginfo("Start pose move could not finish.")
            self.handle_job_fail(job)
            self.set_robot_to_idle()
            return

        result = move_base_client.move(end_pose)

        if not result:
            rospy.loginfo("End pose move could not finish.")
            self.handle_job_fail(job)
            self.set_robot_to_idle()
            return

        self.handle_job_success(job)
        self.set_robot_to_idle()

    def handle_job_success(self, job):
        rospy.loginfo("Handling job success for jobId: " + job.id)
        rospy.loginfo(str(job))
        job.Status = JobStatus.Complete.value
        self.jobs_handler.publish_job(job)

    def handle_job_fail(self, job):
        rospy.loginfo("Handling job failure for jobId: " + job.id)
        rospy.loginfo(str(job))
        job.Status = JobStatus.Failed.value
        self.jobs_handler.publish_job(job)

    def set_robot_to_failed(self):
        self.update_robot_state(RobotStatus.Failed, None)

    def set_robot_to_idle(self):
        self.update_robot_state(RobotStatus.Idle, None)

if __name__ == '__main__':
    try:
        fleet_manager_url = rospy.get_param("fleetmanager_url")
        fleet_manager_version = rospy.get_param("fleetmanager_version")
        robotName = rospy.get_param("robot_name")

        sys.stdout = os.fdopen(sys.stdout.fileno(), 'w', 1)  # Set line buffering

        controller = RobotController(robotName, fleet_manager_url, fleet_manager_version)

    except rospy.ROSInterruptException:
        pass