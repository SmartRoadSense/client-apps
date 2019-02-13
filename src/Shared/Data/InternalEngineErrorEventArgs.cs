using System;
using SmartRoadSense.Core;

namespace SmartRoadSense.Shared.Data {

    public class InternalEngineErrorEventArgs {

        public InternalEngineErrorEventArgs(EngineComputationException exception) {
            Exception = exception;
        }

        public EngineComputationException Exception { get; private set; }

    }

}

