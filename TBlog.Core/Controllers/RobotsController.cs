using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using TBlog.Core.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBlog.Core.Controllers
{
    public class RobotsController : Controller
    {
        private readonly IBlogRepository _blogRepository;

        public RobotsController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        [Route("/robots.txt")]

        // GET: /<controller>/
        public string RobotsTxt()
        {
            string host = Request.Scheme + "://" + Request.Host;

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("User-agent: *");
            stringBuilder.AppendLine("Disallow:");
            stringBuilder.AppendLine($"sitemap: {host}/sitemap.xml");

            return stringBuilder.ToString();
        }


        [Route("/sitemap.xml")]
        public async Task SitemapXml()
        {
            string host = Request.Scheme + "://" + Request.Host;

            Response.ContentType = "application/xml";

            using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                var posts = await _blogRepository.GetPostsAsync(1, int.MaxValue, string.Empty, null);

                foreach (var post in posts)
                {
                    xml.WriteStartElement("url");
                    xml.WriteElementString("loc", host + "/post/" + post.Slug);
                    xml.WriteElementString("lastmod", post.ModifiedDate.ToString("yyyy-MM-ddThh:mmzzz"));
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
        }
    }
}
