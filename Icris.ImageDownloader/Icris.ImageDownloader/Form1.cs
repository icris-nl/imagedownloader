using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Icris.ImageDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.listBox1.DataSource = links;
        }

        List<string> urls = new List<string>();
        BindingList<string> links = new BindingList<string>();
        void GetImages(string url)
        {
            var document = new HtmlWeb().Load(url);
            var images = document.DocumentNode.Descendants("img").ToList();
            urls.AddRange(images
                            .Select(e => e.GetAttributeValue("src", null))
                            .Where(s => !String.IsNullOrEmpty(s)));
            var parents = document.DocumentNode.Descendants("img").Select(x => x.ParentNode.Attributes["href"].Value.Replace("/url?q=", "")).ToList();

            parents.ForEach(item =>
            {
                item = item.Substring(0, item.IndexOf("&amp;"));
                links.Add(item);
            }); ;

            ///url?q=https://www.sinkegroep.nl/expertises/grondverzet/&amp;sa=U&amp;ved=0ahUKEwiU_bvCmcTjAhViyaYKHfgGDVoQwW4IHDAD&amp;usg=AOvVaw2xbgTm-PHhr4U-bEAzZeig

        }
        List<Image> rawimages = new List<Image>();

        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var counter = 0;
            while (counter < 100)
            {
                var uri = $"https://www.google.com/search?q={this.txtSearch.Text}&sout=1&tbm=isch&start={counter}";
                GetImages(uri);
                counter += 20;
            }            
            foreach(var url in urls)
            {
                var img = Image.FromStream(GetStreamFromUrl(url));
                this.imageList1.Images.Add(img);
                this.rawimages.Add(img);
            }

            this.imageList1.ImageSize = new Size(150, 150);
            this.imageList1.ColorDepth = ColorDepth.Depth32Bit;

            lstResults.LargeImageList = this.imageList1;
            lstResults.View = View.LargeIcon;
            for(int i = 0; i<imageList1.Images.Count; i++)
                lstResults.Items.Add(""+i,i);
            
        }

        private void LstResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstResults.SelectedIndices.Count>0)
                this.pbViewer.Image = rawimages[lstResults.SelectedIndices[0]];
        }
    }
}
