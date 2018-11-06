import rospy
import actionlib

from geometry_msgs.msg import Pose
from move_base_msgs.msg import MoveBaseAction, MoveBaseGoal
from actionlib_msgs.msg import GoalStatus

class MoveBaseClient:

    def __init__(self, id, base_frame="map"):

        self.base_frame = base_frame
        self.client = actionlib.SimpleActionClient('move_base', MoveBaseAction)
        self.client.wait_for_server()
        self.id = id
        self.current_goal = None

    def create_pose(self, x, y):
        pose = Pose()
        pose.position.x = x
        pose.position.y = y
        pose.orientation.w = 1.0

        rospy.loginfo(pose)

        return pose

    def move(self, pose):

        rospy.loginfo("Move was called for id: " + self.id)

        goal = MoveBaseGoal()
        goal.target_pose.header.frame_id = self.base_frame
        goal.target_pose.header.stamp = rospy.Time.now()
        goal.target_pose.pose = pose

        self.current_goal = goal

        self.client.send_goal(goal, self.done_callback, self.active_callback) 

        wait = self.client.wait_for_result()
        
        if not wait:
            rospy.logerr("Action server not available!")
            rospy.signal_shutdown("Action server not available!")
        else:
            return self.client.get_state() == GoalStatus.SUCCEEDED

    def active_callback(self):
        rospy.loginfo("Move base goal is active for id: " + self.id)

    def done_callback(self, status, result):

        if (status == GoalStatus.SUCCEEDED):
            rospy.loginfo("Goal succeeded for id: " + self.id)
        
        elif (status == GoalStatus.PREEMPTED or status == GoalStatus.RECALLED):
            rospy.loginfo("Goal was cancelled for id: " + self.id)

        elif (status == GoalStatus.ABORTED):
            rospy.loginfo("Goal aborted for id: " + self.id)
        
        elif (status == GoalStatus.REJECTED):
            rospy.loginfo("Goal rejected for id: " + self.id)
