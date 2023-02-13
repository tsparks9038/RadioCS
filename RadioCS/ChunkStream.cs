using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RadioCS
{
    internal class ChunkStream : WaveStream
    {
        private readonly string _url;
        private readonly int _chunkSize;
        private int _position;

        public ChunkStream(string url, int chunkSize)
        {
            _url = url;
            _chunkSize = chunkSize;
        }

        public override WaveFormat WaveFormat => new WaveFormat(44100, 16, 2);

        public override long Length => int.MaxValue;

        public override long Position
        {
            get => _position;
            set => _position = (int)value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(_url);
            webRequest.AddRange(_position, _position + _chunkSize - 1);

            using (var response = webRequest.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                var bytesRead = responseStream.Read(buffer, offset, count);
                _position += bytesRead;
                return bytesRead;
            }
        }
    }
}
