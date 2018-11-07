// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

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
