using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RadioCS
{
    internal class HttpWaveStream : WaveStream
    {
        private readonly WebResponse response;
        private readonly Stream stream;

        public HttpWaveStream(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            response = request.GetResponse();
            stream = response.GetResponseStream();
        }

        public override WaveFormat WaveFormat => new Mp3WaveFormat(44100, 2, 128, 128000);

        public override long Length => response.ContentLength;

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                stream.Dispose();
                response.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
