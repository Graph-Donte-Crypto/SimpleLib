using System.Net.Sockets;
using System.Text;

namespace Simple {
	public class SocketHelper {
		public TcpClient client;
		public NetworkStream stream;

		private static int zeros = Encoding.UTF8.GetByteCount("00000000");
		private static string NumToZerosString(int num) {
			return string.Format("{0:D8}", num);
		}

		private void sendUnsafeMessage(string message) {
			byte[] data = Encoding.UTF8.GetBytes(message);
			stream.Write(data, 0, data.Length);
		}

		private void sendUnsafeBytesMessage(byte[] message) {
			stream.Write(message, 0, message.Length);
		}
		public void sendMessage(string message) {
			int size = Encoding.UTF8.GetByteCount(message);
			sendUnsafeMessage(NumToZerosString(size));
			sendUnsafeMessage(message);
		}

		public void sendBytesMessage(byte[] message) {
			sendUnsafeMessage(NumToZerosString(message.Length));
			sendUnsafeBytesMessage(message);
		}
		private string receiveUnsafeMessage(int size) {
			byte[] data = new byte[size];
			stream.Read(data, 0, size);
			string returndata = Encoding.UTF8.GetString(data);
			return returndata.Trim(new char[] { '\0' });
		}
		private byte[] receiveUnsafeBytesMessage(int size) {
			byte[] data = new byte[size];
			stream.Read(data, 0, size);
			return data;
		}
		public string receiveMessage() {
			string receive = receiveUnsafeMessage(zeros);
			int size = int.Parse(receive);
			return receiveUnsafeMessage(size);
		}		
		public byte[] receiveBytesMessage() {
			string receive = receiveUnsafeMessage(zeros);
			int size = int.Parse(receive);
			return receiveUnsafeBytesMessage(size);
		}

		public void accept(TcpClient client) {
			this.client = client;
			stream = client.GetStream();
		}

		public void connect(string ip, int port) {
			client = new TcpClient();
			client.Connect(ip, port);
			stream = client.GetStream();
		}

		public void close() {
			if (client.Connected)
				client.Close();
		}
	}
}
