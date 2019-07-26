using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartRoadSense
{
    public class TrackDataRemote : ITrackDataService
    {
        // source name
        readonly static string source = "TRACKDATA REMOTE";

        // resource path
        readonly static string loginResource = ClientConstants.Track.Resource.ResourcePath;

        // header keys
        readonly static string trackDataAuthHeaderKey = ClientConstants.Track.HeaderKey.Authorization;
        readonly static string trackDataAuthHeaderPrefix = ClientConstants.Track.HeaderKey.AuthorizationPrefix;

        // success codes
        readonly static int loginSuccessCode = ClientConstants.Track.Resource.StatusCode.Ok;

        // client
        Client _client;

        public TrackDataRemote(Client client)
        {
            _client = client;
        }

        public async Task<Outcome<string, TrackDataException>> SendTracks(string tracks)
        {
            try
            {
                // Coordinates
                var method = HttpMethod.Post;
                var resource = loginResource + "/endpoint";

                // Headers
                var key = Convert.ToBase64String(Encoding.ASCII.GetBytes("token"));
                var headers = new Dictionary<string, string> { };

                headers.Add(trackDataAuthHeaderKey, trackDataAuthHeaderPrefix + "token");

                // Prepare request
                var body = new Dictionary<string, string>();
                _client.PrepareRequest(method, resource, headers, body);

                //  Send request
                var response = await _client.SendRequest();
                var statusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode && statusCode == loginSuccessCode)
                {

                    var content = await response.Content.ReadAsStringAsync();
                    //var elementContainer = JsonConvert.DeserializeObject<string>(content);

                    // Return outcome
                    var outcome = new Outcome<string, TrackDataException>(content);
                    //var outcome = new Outcome<string, TrackDataException>(elementContainer);
                    return outcome;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new RemoteException(statusCode, message);
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("{0} - {1} - ERROR: \"{2}\"", source, ex.GetType(), ex.Message);
                var internetException = new InternetConnectionException(GenericExceptionErrors.DeviceInternetConnectionErrorMessage);
                var error = new TrackDataException(TrackDataException.Types.nointernet, GenericExceptionErrors.DeviceInternetConnectionErrorTitle + internetException.Message, internetException);
                return new Outcome<string, TrackDataException>(error);
            }
            catch (RemoteException ex)
            {
                Debug.WriteLine("{0} - {1} - ERROR: \"{2}\"", source, ex.GetType(), ex.Message);
                var error = TrackDataException.FromRemoteException(ex);
                return new Outcome<string, TrackDataException>(error);
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine("{0} - {1} - ERROR: \"{2}\"", source, ex.GetType(), ex.Message);
                var error = new TrackDataException(TrackDataException.Types.invalidData, ex.Message);
                return new Outcome<string, TrackDataException>(error);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine("{0} - {1} - ERROR: \"{2}\"", source, ex.GetType(), ex.Message);
                var error = new TrackDataException(TrackDataException.Types.invalidData, ex.Message);
                return new Outcome<string, TrackDataException>(error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} - {1} - ERROR: \"{2}\"", source, ex.GetType(), ex.Message);
                var error = new TrackDataException(TrackDataException.Types.generic, ex.Message);
                return new Outcome<string, TrackDataException>(error);
            }
        }
    }
}
