using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SmartRoadSense.Shared.Data {

    public class DataReader : IDisposable {

        private readonly Guid _trackId;
        private readonly StreamReader _reader;
        private DataLine _currentLine;

        public DataReader(Guid trackId) {
            _trackId = trackId;

            var filepath = FileNaming.GetDataTrackFilepath(trackId);
            _reader = new StreamReader(FileOperations.ReadFile(filepath));
        }

        public void Dispose() {
            _reader.Dispose();
        }

        public void Reset() {
            Row = -1;
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            _reader.DiscardBufferedData();
        }

        public int Row { get; private set; } = -1;

        public ref DataLine Current {
            get {
                if(Row < 0) {
                    throw new InvalidOperationException();
                }

                return ref _currentLine;
            }
        }

        public async Task<bool> Skip(int skip) {
            if(skip < 0) {
                throw new ArgumentException();
            }

            while(skip > 0) {
                var line = await _reader.ReadLineAsync();
                if(line == null) {
                    return false;
                }
                if(line[0] == '#') {
                    // Skip comments
                    continue;
                }

                skip--;
            }
            return true;
        }

        public async Task<bool> Advance() {
            while(true) {
                var line = await _reader.ReadLineAsync();
                if(line == null) {
                    return false;
                }
                if(line[0] == '#') {
                    // Skip comments
                    continue;
                }

                Row++;
                await Task.Run(() => { ParseLine(line); });

                return true;
            }
        }

        private void ParseLine(string line) {
            try {
                var fields = line.Split(new char[] { ',' }, StringSplitOptions.None);
                _currentLine = new DataLine(
                    long.Parse(fields[0], CultureInfo.InvariantCulture),
                    long.Parse(fields[1], CultureInfo.InvariantCulture),
                    double.Parse(fields[2], CultureInfo.InvariantCulture),
                    double.Parse(fields[3], CultureInfo.InvariantCulture),
                    double.Parse(fields[4], CultureInfo.InvariantCulture),
                    double.Parse(fields[5], CultureInfo.InvariantCulture),
                    double.Parse(fields[6], CultureInfo.InvariantCulture),
                    double.Parse(fields[7], CultureInfo.InvariantCulture),
                    double.Parse(fields[8], CultureInfo.InvariantCulture),
                    int.Parse(fields[9], CultureInfo.InvariantCulture),
                    int.Parse(fields[10], CultureInfo.InvariantCulture)
                );
            }
            catch(Exception ex) {
                Log.Error(ex, "Line parsing error");
            }
        }
    }

}
