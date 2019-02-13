using System;

using SmartRoadSense.Core;

namespace SmartRoadSense.Shared.Data {

    public class DataPointRecordedEventArgs : EventArgs {

        public DataPointRecordedEventArgs(DataPiece data, SessionInfo session, Result result) {
            Data = data;
            Session = session;
            ComputationResult = result;
        }

        public DataPiece Data { get; private set; }

        public SessionInfo Session { get; private set; }

        public Result ComputationResult { get; private set; }

    }

}

