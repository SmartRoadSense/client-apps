using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoadSense.Toolkit {

    internal class OutputWrapper : IDisposable {

        public OutputWrapper(TextWriter writer) {
            Writer = writer;
        }

        public OutputWrapper(Stream output) {
            RawStream = output;
            Writer = new StreamWriter(output);
        }

        Stream _rawStream = null;

        public Stream RawStream {
            get {
                if(_rawStream == null) {
                    throw new InvalidOperationException("Output target cannot write raw bytes (i.e., cannot write to standard output)");
                }
                else {
                    return _rawStream;
                }
            }
            private set {
                _rawStream = value;
            }
        }

        public bool HasRawStream {
            get {
                return _rawStream != null;
            }
        }

        public TextWriter Writer { get; private set; }

        ~OutputWrapper() {
            Close();
        }

        public void Dispose() {
            Close();

            GC.SuppressFinalize(this);
        }

        public void Close() {
            if (Writer != null) {
                Writer.Dispose();
                Writer = null;
            }
            if (_rawStream != null) {
                _rawStream.Dispose();
                _rawStream = null;
            }
        }

    }

}
