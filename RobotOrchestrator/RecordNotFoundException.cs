﻿using System;

namespace RobotOrchestrator
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException()
        {
        }

        public RecordNotFoundException(string message)
            : base(message)
        {
        }

        public RecordNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
