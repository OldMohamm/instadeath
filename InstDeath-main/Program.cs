using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Web;

class Program
{
	public static string label = @"
    __    __           __   __          __   
   / /_  / /___ ______/ /__/ /_  ____  / /__ 
  / __ \/ / __ `/ ___/ //_/ __ \/ __ \/ / _ \
 / /_/ / / /_/ / /__/ ,< / / / / /_/ / /  __/
/_.___/_/\__,_/\___/_/|_/_/ /_/\____/_/\___/ 
                                             
By Hades @0nlymohammed
";
	public static Dictionary<string, string> Responses = new Dictionary<string, string>();
	public static List<string> Proxies = new List<string>();
	public static string body = null;
	public static string[] Usernames = null;
	public static int BadProxies = 0;
	public static int Wait = 0;
	public static int Errors = 0;
	public static int UsernameIndex = 0;
	public static bool Lap = false;
	public static string Password = null;
	public static string Email = null;
	public static bool randomEndpoint = false;
	public static int ThreadsPerSecond = 0;
	public static int EndpointSwitch = 0;
	public static string ProxiesType = null;
	public static string LastClaim = ".";
	public static int RequestHandled = 0;
	public static Random random = new Random();
	public static void check(object proxy, object user, string endpoint=null) {
		string Endpoint = null;
		if (endpoint != null) {
			Endpoint = endpoint;
		}
		try {

			WebHeaderCollection headers = new WebHeaderCollection();
			headers.Add("User-Agent", $"Instagram {random.Next(5, 50)}.{random.Next(6, 10)}.{random.Next(0, 10)} Android (18/2.1; 160dpi; 720x900; ZTE; LAVA-9L7EZ; pdfz; hq3143; en_US)");
			headers.Add("Accept", "*/*");
			headers.Add("Cookie2", "$Version=1");
			headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
			headers.Add("X-IG-Connection-Type", "WIFI");
			headers.Add("Accept-Language", "en-US");
			headers.Add("X-FB-HTTP-Engine", "Liger");
			headers.Add("X-IG-Capabilities", "3brTBw==");
			headers.Add("Proxy-Connection", "Keep-Alive");
			headers.Add("Cookie", $"ds_user_id={Usernames[(int)user]};");
		
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add("email", $"{Email.Split('@')[0]}+{random.Next(111111,999999)}@{Email.Split('@')[1]}");
			data.Add("phone_id", Guid.NewGuid().ToString());
			data.Add("enc_password", $"#PWD_INSTAGRAM_BROWSER:0:{(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds}:{Password}");
			data.Add("_csrftoken", "missing");
			data.Add("username", Usernames[(int)user]);
			data.Add("adid", Guid.NewGuid().ToString());
			data.Add("device_id", $"android-{RandomString(16)}");
			data.Add("guid", Guid.NewGuid().ToString());
			data.Add("first_name", $"{RandomString(10)}");

			string JSON = DictToJSON(data);
			body = $"signed_body=SIGNATURE.{JSON}";

			if (!randomEndpoint && endpoint == null) {
				if (EndpointSwitch == 0) {
				Endpoint = "create";
				} else if (EndpointSwitch == 1) {
					Endpoint = "create_business";
				} else { EndpointSwitch = 0; }
			} else if (endpoint != null) {
				int rnd = random.Next(0, 1);
				if (rnd == 0) Endpoint = "create";
				if (rnd == 1) Endpoint = "create_business";
			}
			
			HttpWebRequest request = WebRequest.CreateHttp($"https://i.instagram.com/api/v1/accounts/{Endpoint}/");
			request.Method = "POST";
			request.KeepAlive = false;
			request.Headers = headers;
			request.ProtocolVersion = HttpVersion.Version10;
			request.ServicePoint.UseNagleAlgorithm = false;
			request.ServicePoint.Expect100Continue = false;
			//request.ServicePoint.SetTcpKeepAlive(true, 900000000, 100000000);
			request.Timeout = 5000;
			request.Proxy = new WebProxy(Proxies[(int)proxy].Split(':')[0], Int32.Parse(Proxies[(int)proxy].Split(':')[1]));

			byte[] bytes = Encoding.ASCII.GetBytes(body);
			request.ContentLength = (long)bytes.Length;
			Stream StreamWriter = request.GetRequestStream();
			StreamWriter.Write(bytes, 0, bytes.Length);
			StreamWriter.Flush();
			StreamWriter.Close();
			HttpWebResponse HttpResponse = (HttpWebResponse)request.GetResponse();
			StreamReader Reader = new StreamReader(HttpResponse.GetResponseStream());
			string Response = Reader.ReadToEnd();
			Reader.Close();
			string log = "NULL";
			if (Response.Contains("html")) {
				log = $"{HttpResponse.StatusCode}\n{Proxies[(int)proxy]}\n{Usernames[(int)user]}\n{Endpoint}";
			} else {
				log = $"{Response}\n{Proxies[(int)proxy]}\n{Usernames[(int)user]}\n{Endpoint}";
			}
			
			StreamWriter _writer = File.AppendText("log.txt");
			_writer.WriteLine(log);
			_writer.Close();


			if (Response.Contains("challenge_required") ||Response.Contains("checkpoint_required") || Response.Contains("account_created\": true")) {
				StreamWriter writer = File.AppendText("claims.txt");
				writer.WriteLine($"{Usernames[(int)user]}:{Password}");
				writer.Close();
				LastClaim = Usernames[(int)user];
			} else if (Response.ToLower().Contains("held")) {
				StreamWriter writer = File.AppendText("14day.txt");
				writer.WriteLine($"{Usernames[(int)user]}");
				writer.Close();
			}	
			RequestHandled++;
		} catch (System.IndexOutOfRangeException) {
			if (UsernameIndex > Usernames.Length) {
				UsernameIndex = 0;
			}
		} catch (WebException ex) {
			if (!ex.Message.Contains("timed out") && !ex.Message.Contains("Bad Gateway") && !ex.Message.Contains("502") && !ex.Message.Contains("connect")) {
				HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
				StreamReader Reader = new StreamReader(errorResponse.GetResponseStream());
				string Response = Reader.ReadToEnd();
				Reader.Close();
				string log = "NULL";
				if (Response.Contains("html")) {
					log = $"{errorResponse.StatusCode}\n{Proxies[(int)proxy]}\n{Usernames[(int)user]}\n{Endpoint}";
				} else {
					log = $"{Response}\n{Proxies[(int)proxy]}\n{Usernames[(int)user]}\n{Endpoint}";
				}
				StreamWriter _writer = File.AppendText("log.txt");
				_writer.WriteLine(log);
				_writer.Close();

				if (Response.ToLower().Contains("spam")) {
					BadProxies++;
					Proxies.Remove(Proxies[(int)proxy]);
				} else if (Response.ToLower().Contains("wait")) {
					Wait++;
				}
			} else {
				StreamWriter r = File.AppendText("errors_log.txt");
				r.WriteLine($"{ex.Message} : {ex.GetType().toString()}");
				r.Close();
			}
			int NextProxy = 0;
			if ((int)proxy == Proxies.Count - 1) {
				NextProxy = random.Next(Proxies.Count);
				check(NextProxy, user);
			} else {
				check((int)proxy + 1, user);
			}
		} catch (Exception ex) { 
			throw ex;
		}
	}

	public static void SuperVisior() {
		Console.Clear();
		while (true) {
			if (Lap || Wait >= Proxies.Count - 10) {
				EndpointSwitch++;
				Lap = false;
			}
			Console.SetCursorPosition(1,0);
			Console.Write($"\n");
			Console.Write($"{label}\n");
			Console.Write($"Counter: {RequestHandled}\n");
			Console.Write($"Bad Proxies: {BadProxies}\n");
			Console.Write($"Wait: {Wait}\n");
			Console.Write($"Errors: {Errors}\n");
			Console.WriteLine($"Last Claim: {LastClaim}");
			Thread.Sleep(250);
		}
	}
	private static void Main(string[] args)
	{

		//بعثر البروكسيات واليوزرات؟
		//timeout > recheck

		var stdout = Console.OpenStandardOutput();
		var con = new StreamWriter(stdout, Encoding.ASCII);
		con.AutoFlush = true;
		Console.SetOut(con);

		Console.Clear();
		Console.Write("Enter the Usernames list: ");
		Usernames = File.ReadAllLines(Console.ReadLine());

		Console.Write("Enter the proxy list: ");
		Proxies = new List<string>(File.ReadAllLines(Console.ReadLine()));
		Shuffle(Proxies);
		Proxies.Insert(0, "null");

		Console.Write("Enter threads: ");
		ThreadsPerSecond = int.Parse(Console.ReadLine());

		Console.Write("Enter email: ");
		Email = Console.ReadLine();

		Console.Write("Enter password: ");
		Password = Console.ReadLine();

		bool once = true;
		randomEndpoint = true;
		
		List<Thread> threads = new List<Thread>();
		int ProxyGiversLimit = 10;
		int ProxyGivers = 0;
		if (ThreadsPerSecond > ProxyGiversLimit)
		{
			for (int k = 0; k <= ThreadsPerSecond; k++)
			{
				if (k != 0 && k % ProxyGiversLimit == 0)
				{
					ProxyGivers++;
					if (ThreadsPerSecond - ProxyGivers * ProxyGiversLimit < ProxyGiversLimit)
					{
						break;
					}
				}
			}
			int left = ThreadsPerSecond - ProxyGivers * ProxyGiversLimit;
			for (int j = 0; j <= ProxyGivers; j++)
			{
				Thread t3 = new Thread((ParameterizedThreadStart)delegate(object i)
				{
					int index3 = (int)i + 1;
					while (true)
					{
						List<Thread> list3 = new List<Thread>();
						for (int n = 0; n <= ProxyGiversLimit; n++) {
							Thread thread3 = new Thread((ParameterizedThreadStart)delegate(object c) {
								check((int)index3, (int)c);
							}) {
								IsBackground = true,
								Priority = ThreadPriority.Highest
							};
							list3.Add(thread3);
							thread3.Start(UsernameIndex);
							if (once) { index3 += ProxyGivers + 1; } //???
							if (index3 > Proxies.Count - 1) {
								index3 = (int)i + 1;
								Lap = true;
							}
							UsernameIndex++;
						}
						foreach (Thread current3 in list3) {
							current3.Join();
						}
						//index3 += ProxyGivers;
						if (!once) { index3 += ProxyGivers + 1; }
						if (index3 > Proxies.Count - 1) {
							index3 = (int)i + 1;
							Lap = true;
						}
					}
				});
				t3.IsBackground = true;
				t3.Priority = ThreadPriority.AboveNormal;
				threads.Add(t3);
				t3.Start(j);
			}
			if (left > 0)
			{
				Thread t4 = new Thread((ParameterizedThreadStart)delegate(object i)
				{
					int index2 = (int)i + 1;
					while (true)
					{
						List<Thread> list2 = new List<Thread>();
						for (int m = 0; m <= left; m++)
						{
							Thread thread2 = new Thread((ParameterizedThreadStart)delegate(object c)
							{
								check((int)index2, (int)c);
							})
							{
								IsBackground = true,
								Priority = ThreadPriority.Highest
							};
							list2.Add(thread2);
							thread2.Start(UsernameIndex);
							if (once) { index2 += ProxyGivers + 1; }
							if (index2 > Proxies.Count - 1) {
								index2 = (int)i + 1;
								Lap = true;
							}
							UsernameIndex++;
						}
						foreach (Thread current2 in list2)
						{
							current2.Join();
						}
						//index2 += ProxyGivers;
						if (!once) { index2 += ProxyGivers + 1; }
						if (index2 > Proxies.Count - 1) {
							index2 = (int)i + 1;
							Lap = true;
						}
					}
				});
				t4.IsBackground = true;
				t4.Priority = ThreadPriority.AboveNormal;
				threads.Add(t4);
				t4.Start(ProxyGivers + 1);
			}
		}
		else
		{
			Thread t2 = new Thread((ParameterizedThreadStart)delegate(object i)
			{
				int index = (int)i + 1;
				while (true)
				{
					
					List<Thread> list = new List<Thread>();
					for (int l = 0; l <= ThreadsPerSecond; l++)
					{
						Thread thread = new Thread((ParameterizedThreadStart)delegate(object c)
						{
							check(index, (int)c);
						})
						{
							IsBackground = true,
							Priority = ThreadPriority.Highest
						};
						list.Add(thread);
						thread.Start(UsernameIndex);
						if (once) { index += ProxyGivers + 1; }
						if (index > Proxies.Count - 1) {
							index = (int)i + 1;
							Lap = true;
						}
						UsernameIndex++;
					}
					foreach (Thread current in list)
					{
						current.Join();
					}
					//index += ProxyGivers;
					if (!once) { index += ProxyGivers + 1; }
					//Console.WriteLine(Proxies[(int)index].Split(':')[0] + ":" + Int32.Parse(Proxies[(int)index].Split(':')[1]));
					if (index > Proxies.Count - 1) {
						index = (int)i + 1;
						Lap = true;
					}
				}
			});
			t2.IsBackground = true;
			t2.Priority = ThreadPriority.AboveNormal;
			threads.Add(t2);
			t2.Start(0);
		}
		Thread _t = new Thread(() => {
			SuperVisior();
		});
		_t.Priority = ThreadPriority.BelowNormal;
		_t.Start();
		foreach (Thread t in threads)
		{
			t.Join();
		}
	}

	private static string DictToJSON(Dictionary<string, string> dict)
	{
		int i = 0;
		string json = "{";
		foreach (KeyValuePair<string, string> entry in dict)
		{
			json = ((i != dict.Count - 1) ? (json + "\"" + entry.Key + "\":\"" + entry.Value + "\",") : (json + "\"" + entry.Key + "\":\"" + entry.Value + "\""));
			i++;
		}
		return json + "}";
	}

	private static string RandomString(int length)
	{
		return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select((Func<string, char>)((string s) => s[random.Next(s.Length)])).ToArray());
	}
	public static String betweenStrings(String text, String start, String end)
    {
        int p1 = text.IndexOf(start) + start.Length;
        int p2 = text.IndexOf(end, p1);

        if (end == "") return (text.Substring(p1));
        else return text.Substring(p1, p2 - p1);                      
    }
	public static void Shuffle<T>(IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = random.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

}

