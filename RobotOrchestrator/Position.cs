// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace RobotOrchestrator
{
    public class Position
    {
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}
