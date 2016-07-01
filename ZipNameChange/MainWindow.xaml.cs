using System;
using System.IO;
using System.IO.Compression; 
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace ZipNameChange
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog fbDialog = new FolderBrowserDialog();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDirDialog_Click(object sender, RoutedEventArgs e)
        {
            if(fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strDir = fbDialog.SelectedPath;
                tbFirPath.Text = strDir;

                GetZipList(strDir);
            }  
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            ChangeZipName();
        }

        private void GetZipList(string strDir)
        {
            DirectoryInfo di = new DirectoryInfo(strDir);
            FileInfo[] fis = di.GetFiles("*.zip");

            int iFileCnt = fis.Length;

            listView.BeginInit();
            //listView.Items.Clear();

            for (int i = 0; i < iFileCnt; i++)
            {
                GetZipInfo(fis[i]);
            }

            listView.EndInit();
        }

        private void GetZipInfo(FileInfo fi)
        {
            try
            {
                bool bCheckflag = false;

                ZipArchive zipa = ZipFile.OpenRead(fi.FullName);

                string strZipDetail = zipa.Entries[0].FullName;

                int iSepPoint = strZipDetail.IndexOf("/");

                if (iSepPoint != -1)
                {
                    string strTopDir = strZipDetail.Substring(0, iSepPoint);
                    if (System.IO.Path.GetFileNameWithoutExtension(fi.Name).Equals(strTopDir) == false)
                    {
                        bCheckflag = true;
                        int iNo = listView.Items.Add(new ZipListItem() { IsSelected = bCheckflag, ZipName = fi.Name, Folder = strTopDir });  
                    }

                    //int iNo = listView.Items.Add(new ZipListItem() { IsSelected = bCheckflag, ZipName = fi.Name, Folder = strTopDir });  

                }
                zipa.Dispose();

            }
            catch (InvalidDataException ide)
            {

            }
        }

        private void ChangeZipName()
        {
            bool bFlag = true;

            string strDirName = tbFirPath.Text;
            int iChkCnt = listView.Items.Count;

            string strIncName = "";
            int iInc = 0;

            for (int i = 0; i < iChkCnt; i++)
            {
                ZipListItem zipa = (ZipListItem)listView.Items[i];

                if (zipa.IsSelected == false)
                {
                    continue;
                }
                strIncName = "";
                iInc = 0;
                FileInfo fi = new FileInfo(strDirName + "\\" + zipa.ZipName);
                while (bFlag)
                {
                    FileInfo fiChg = new FileInfo(strDirName + "\\" + zipa.Folder + strIncName + ".zip");
                    if (fiChg.Exists == false)
                    {
                        try
                        {
                            fi.MoveTo(strDirName + "\\" + zipa.Folder + strIncName + ".zip");
                        }
                        catch (IOException ioe)
                        {

                        }
                        break;
                    }
                    else
                    {
                        iInc++;
                        strIncName = "_" + iInc.ToString();
                    }
                }
            }

            GetZipList(strDirName);
        }
    }

    public class ZipListItem
    {
        public bool IsSelected { get; set;}
        public string ZipName { get; set; }
        public string Folder { get; set; }
    }
}
