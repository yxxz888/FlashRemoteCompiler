using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlashRemoteCompilerServer
{
    class ClientObject
    {
        public String name;
        public TcpClient client;
        public byte[] buffer;
        public String message;
        private String[] _fileList = new String[0];
        private String[] _flaList = new String[0];

        public ClientObject(TcpClient client)
        {
            this.client = client;
            resetBuffer();
        }


        public void resetBuffer()
        {
            buffer = new byte[128];
        }


        public void clearMessage()
        {
            message = "";
        }


        public Boolean isFinished()
        {
            return _fileList.Length == 0;
        }


        public void finishCompile()
        {
            _fileList = new String[0];
        }


        public string[] fileList
        {
            get
            {
                return _fileList;
            }

            set
            {
                _fileList = value;

                List<String> list = new List<String>();
                foreach ( String file in _fileList)
                {
                    if (new FileInfo(file).Extension == ".fla")
                        list.Add(file);
                }
                _flaList = list.ToArray();
            }
        }


        public string[] flaList
        {
            get
            {
                return _flaList;
            }
        }
    }
}
