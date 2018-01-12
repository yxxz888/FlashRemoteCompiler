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
        public String[] uploadFileList;

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


        public String[] fileList
        {
            get
            {
                return _fileList;
            }

            set
            {
                _fileList = new String[value.Length];
                List<String> list = new List<String>();
                for (int i = 0;i < value.Length;i++)
                {
                    _fileList[i] = Path.Combine(ConfigInfo.assets, value[i]);

                    if(new FileInfo(_fileList[i]).Extension == ".fla")
                        list.Add(_fileList[i]);
                }
                _flaList = list.ToArray();                
            }
        }


        public String[] flaList
        {
            get
            {
                return _flaList;
            }
        }
    }
}
