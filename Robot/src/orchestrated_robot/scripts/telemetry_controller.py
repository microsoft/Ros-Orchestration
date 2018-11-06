#!/usr/bin/env python

# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

import rospy
from datetime import datetime
from nav_msgs.msg import Odometry
from orchestrator_msgs.msg import Telemetry
from geometry_msgs.msg import Pose
from orchestrator_msgs.srv import RobotInfo

class TelemetryController:

    def __init__(self):
        rospy.init_node('telemetry_controller', anonymous=True)

        self.telemetry_publisher = self.set_up_telemetry_publisher()
        self.pose_subscriber = self.set_up_pose_subscriber()
        self.currentPose = Pose()
        self.publish_latest_telemetry(self.telemetry_publisher)

    def set_up_telemetry_publisher(self):
        """ create publisher for fake jobs test messages """
        pub = rospy.Publisher('telemetry', Telemetry, queue_size=10)
        return pub

    def set_up_pose_subscriber(self):
        """ create subscriber for pose messages """
        sub = rospy.Subscriber('odom', Odometry, self.update_current_pose)
        return sub

    def update_current_pose(self, odom_data):
        self.currentPose = odom_data.pose.pose

    def publish_latest_telemetry(self, pub):

        # connect to robot controller service
        get_robot_info = rospy.ServiceProxy('get_robot_info', RobotInfo)

        while not rospy.is_shutdown():
            utctime = datetime.utcnow().isoformat()

            telemetry = Telemetry()

            # get robot info from robot controller
            resp = get_robot_info()
            robotId = resp.RobotId
            robotStatus = resp.Status
            currentOrder = resp.OrderId

            # create current telemetry
            telemetry.OrderId = currentOrder
            telemetry.Position.X = self.currentPose.position.x
            telemetry.Position.Y = self.currentPose.position.y
            telemetry.id = utctime
            telemetry.Status = robotStatus
            telemetry.RobotId = robotId
            telemetry.CreatedDateTime = utctime

            #rospy.loginfo(telemetry)

            # publish 1 time per second
            rate = rospy.Rate(1)

            pub.publish(telemetry)
            rate.sleep()

if __name__ == '__main__':

    rospy.wait_for_service('get_robot_info')
    telemetryController = TelemetryController()