from enum import Enum

class RobotStatus(Enum):
    Onboarding = "Onboarding"
    Idle = "Idle"
    Busy = "Busy"
    Failed = "Failed"