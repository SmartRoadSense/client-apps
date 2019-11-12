using System;
using SmartRoadSense.Core;

namespace SmartRoadSense {

    public class InternalEngineErrorEventArgs {

        public InternalEngineErrorEventArgs(EngineComputationException exception) {
            Exception = exception;
        }

        public EngineComputationException Exception { get; private set; }

    }

}

