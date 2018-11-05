import rospy
from orchestrator_msgs.msg import Job

class JobsHandler:

    def __init__(self, robot_controller):
        self.robot_controller = robot_controller
        self.jobs_subscriber = self.set_up_jobs_subscriber()
        self.jobs_publisher = self.set_up_jobs_publisher()

    def set_up_jobs_subscriber(self):
        """ create subscriber for jobs """
        sub = rospy.Subscriber('jobs', Job, self.job_callback)
        return sub

    def set_up_jobs_publisher(self):
        """ create publisher for jobs messages """
        pub = rospy.Publisher('jobsstatus', Job, queue_size=10)
        return pub

    def publish_job(self, job):
        """ publish job to topic """
        self.jobs_publisher.publish(job)

    def job_callback(self, data):
        """ callback for job topic """
        rospy.loginfo(" Received a job for %s", data.RobotId)
        self.robot_controller.handle_job_new(data)