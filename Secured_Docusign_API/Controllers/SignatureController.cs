using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;

using System.Web.Http.Cors;

namespace Secured_Docusign_API.Controllers
{
	[EnableCors(origins: "*",
		headers: "*",
		methods: "*",
		SupportsCredentials = true)]
	[Authorize]
	public class SignatureController : ApiController
	{


		private string accessToken;
		private const string accountId = "11657009";
		//private const string signerName = "asif";
		//private const string signerEmail = "asif@resemblesystems.com";
		string signerName;
		string signerEmail;
		private const string docName = "World_Wide_Corp_lorem.pdf";
		private const string signerClientId = "1000";
		protected static ApiClient _apiClient { get; private set; }
		private static Account _account { get; set; }
		private const string basePath = "https://demo.docusign.net/restapi";
		private const string returnUrl = "https://dmcs-dev.itfc-idb.org/apps/Memo/Pages/TwoFactorAuthentication.aspx";


		[HttpGet]
		public async Task<DocuSign.eSign.Model.ViewUrl> sendenvelope(string signerName, string signerEmail)
		{
			//public async Task<DocuSign.eSign.Model.ViewUrl> sendenvelope(string signerName, string signerEmail)
			//var signerName =obj.name;
			///var signerEmail = obj.email;
			Document document = new Document
			{
				DocumentBase64 = Convert.ToBase64String(ReadContent(docName)),
				Name = "Lorem Ipsum",
				FileExtension = "pdf",
				DocumentId = "1"
			};
			Document[] documents = new Document[] { document };



			accessToken = GetAccessToken();

			// Create the signer recipient object 
			Signer signer = new Signer
			{
				Email = signerEmail,
				Name = signerName,
				ClientUserId = signerClientId,
				RecipientId = "1",
				RoutingOrder = "1"

			};

			// Create the sign here tab (signing field on the document)
			SignHere signHereTab = new SignHere
			{
				DocumentId = "1",
				PageNumber = "1",
				RecipientId = "1",
				TabLabel = "Sign Here Tab",
				XPosition = "195",
				YPosition = "147"
			};
			SignHere[] signHereTabs = new SignHere[] { signHereTab };

			// Add the sign here tab array to the signer object.
			signer.Tabs = new Tabs { SignHereTabs = new List<SignHere>(signHereTabs) };
			// Create array of signer objects
			Signer[] signers = new Signer[] { signer };
			// Create recipients object
			Recipients recipients = new Recipients { Signers = new List<Signer>(signers) };
			// Bring the objects together in the EnvelopeDefinition
			EnvelopeDefinition envelopeDefinition = new EnvelopeDefinition
			{
				EmailSubject = "ITFC Memo Approval",
				Documents = new List<Document>(documents),
				Recipients = recipients,
				Status = "sent"
			};

			// 2. Use the SDK to create and send the envelope
			ApiClient apiClient = new ApiClient(basePath);
			apiClient.Configuration.AddDefaultHeader("Authorization", "Bearer " + accessToken);
			EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
			EnvelopeSummary results = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);
			//ViewData["results"] = $"Envelope status: {results.Status}. Envelope ID: {results.EnvelopeId}";

			string envelopeId = results.EnvelopeId;
			RecipientViewRequest viewOptions = new RecipientViewRequest
			{
				ReturnUrl = returnUrl,
				ClientUserId = signerClientId,
				AuthenticationMethod = "none",
				UserName = signerName,
				Email = signerEmail
			};

			// Use the SDK to obtain a Recipient View URL
			ViewUrl viewUrl = envelopesApi.CreateRecipientView(accountId, envelopeId, viewOptions);

			return viewUrl;
		}


		internal static byte[] ReadContent(string fileName)
		{
			byte[] buff = null;
			string directory = Directory.GetCurrentDirectory();
			string path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", fileName);
			//string path = "C:\\Resources";
			using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(stream))
				{
					long numBytes = new FileInfo(path).Length;
					buff = br.ReadBytes((int)numBytes);
				}
			}

			return buff;
		}


		private string GetAccessToken()
		{
			ApiClient _apiClient = new ApiClient();

			var scopes = new List<string>
				{
					"signature",
					"impersonation",
				};
			string ClientId = ConfigurationManager.AppSettings["ClientId"];
			string ImpersonatedUserId = ConfigurationManager.AppSettings["ImpersonatedUserId"];
			string AuthServer = ConfigurationManager.AppSettings["AuthServer"];
			string PrivateKeyFile = ConfigurationManager.AppSettings["PrivateKeyFile"];

			OAuthToken _authToken = _apiClient.RequestJWTUserToken(ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["ImpersonatedUserId"], ConfigurationManager.AppSettings["AuthServer"],

			 DSHelper.ReadFileContent(DSHelper.PrepareFullPrivateKeyFilePath(PrivateKeyFile)), 1, scopes);
			return _authToken.access_token;
		}

	}
}
