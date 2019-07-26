using System;
namespace SmartRoadSense
{
    public class TrackDataException : Exception
    {
        public enum Types { invalidUser, unauthorized, authFailed, invalidData, nointernet, generic, inherited, canceled };

        public Types Type { private set; get; }

        protected TrackDataException() { }

        public TrackDataException(Types type) : base(string.Format("Error type {0}", type))
        {
            Type = type;
        }
        public TrackDataException(Types type, string message) : base(message)
        {
            Type = type;
        }
        public TrackDataException(string message, Exception innerException) : base(message, innerException)
        {
            Type = Types.inherited;
        }

        public TrackDataException(Types type, string message, Exception innerException) : base(message, innerException)
        {
            Type = type;
        }

        static public TrackDataException FromRemoteException(RemoteException exception)
        {
            var code = exception.StatusCode;
            switch (code)
            {
                case ClientConstants.Track.Resource.StatusCode.Malformed:
                    return new TrackDataException(Types.invalidData, exception.Message);
                case ClientConstants.Track.Resource.StatusCode.Unauthorized:
                    return new TrackDataException(Types.unauthorized, exception.Message);
                default:
                    return new TrackDataException(Types.generic, exception.Message);
            }
        }
    }
}
