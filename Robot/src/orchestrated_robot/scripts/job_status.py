from enum import Enum

class JobStatus(Enum):
    Queued = "Queued"
    InProgress = "InProgress"
    Complete = "Complete"
    Failed = "Failed"