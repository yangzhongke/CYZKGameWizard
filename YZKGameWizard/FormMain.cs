using Microsoft.Win32;
using System.Text;
using Vanara.PInvoke;

namespace YZKGameWizard
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void Link_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            string url = (string)label.Tag;
            Shell32.ShellExecute(this.Handle, "open", url, null, null, ShowWindowCommand.SW_NORMAL);
        }

        private void btnBrowseProjectDir_Click(object sender, EventArgs e)
        {
            if(dirDlgProjectPath.ShowDialog(this) == DialogResult.OK)
            {
                txtProjectDir.Text = dirDlgProjectPath.SelectedPath;
            }
        }

        void WriteToFile(string content, FileInfo fName)
        {
            if (!fName.Directory.Exists)
            {
                fName.Directory.Create();
            }
            string lowerContent = content.ToLower();
            bool isUtf8 = lowerContent.Contains("encoding=\"utf-8\"") || lowerContent.Contains("encoding=\"utf8\"");//��ͬ�汾VS
            Encoding encoding;
            if(isUtf8)
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding=Encoding.Default;//VS2008�ȵ��ļ�����ansi��ʽ����֧��utf8
            }
            using var fileStream = fName.OpenWrite();
            using var streamWriter = new StreamWriter(fileStream, encoding);
            streamWriter.Write(content);
        }

		void CopyDir(string dirSrc,string dirDest)
        {
			CopyDir(new DirectoryInfo(dirSrc), new DirectoryInfo(dirDest));
        }

        void CopyDir(DirectoryInfo dirSrc, DirectoryInfo dirDest)
        {
            foreach (var dir in dirSrc.GetDirectories("*",SearchOption.AllDirectories))
            {
                var newDir = new DirectoryInfo(Path.Combine(dirDest.FullName,dir.FullName.Substring(dirSrc.FullName.Length+1)));
                if(!newDir.Exists)
                    newDir.Create();//> C:\sources (and not C:\E:\sources)   
            }

            foreach (var srcFile in dirSrc.GetFiles("*", SearchOption.AllDirectories))
            {
                FileInfo destFile = new FileInfo(Path.Combine(dirDest.FullName,
                    srcFile.FullName.Substring(dirSrc.FullName.Length + 1)));
				destFile.Directory.Create();
				srcFile.CopyTo(destFile.FullName);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
			string projectDir = txtProjectDir.Text;
			string projectName = txtProjectName.Text;

			if (string.IsNullOrWhiteSpace(projectName))
			{
				MessageBox.Show(this,"����д��Ŀ����");
				txtProjectName.Focus();
				return;
			}

			var forbiddenChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());
			foreach (char forbiddenChar in forbiddenChars)
			{
				if (projectName.Contains(forbiddenChar))
				{
					MessageBox.Show(this,"��Ŀ�����в��ܰ���/ ? : & \\ * \" < > | #���������");
					txtProjectName.Focus();
					return;
				}
			}

			if (string.IsNullOrWhiteSpace(projectDir))
			{
				MessageBox.Show(this,"��ѡ����Ŀ·��");
				txtProjectDir.Focus();
				return;
			}

			DirectoryInfo dir = new DirectoryInfo(Path.Combine(projectDir, projectName));
			if (dir.Exists)
			{
				MessageBox.Show(this,projectName + "��Ŀ�Ѿ����ڣ����޸���Ŀ��");
				return;
			}

			string exeDir = Application.StartupPath;
			string templateDir = exeDir + "/��Ŀģ��/";

			FileInfo slnFileNameSrc;//�������ģ��Դ�ļ�
			FileInfo projFileNameSrc;//��Ŀ�ļ�ģ��Դ�ļ�

			FileInfo slnFileNameDest;//�������ģ��Ŀ���ļ�
			FileInfo projFileNameDest;//��Ŀ�ļ�ģ��Ŀ���ļ�

			string ideVer = (string)cmbIDEVersion.SelectedItem;
			switch (ideVer)
			{
				case "VC6":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "vc6.dsw"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "vc6.dsp"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir, projectName, projectName + ".dsp"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir, projectName, projectName + ".dsw"));
					break;
				case "VS2008":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2008.sln"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2008.vcproj"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir,projectName,projectName + ".vcproj"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir,projectName ,projectName + ".sln"));
					break;
				case "VS2010":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2010.sln"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2010.vcxproj"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir,projectName,projectName + ".vcxproj"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir,projectName,projectName + ".sln"));
					break;
				case "VS2012":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2012.sln"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2012.vcxproj"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir,projectName,projectName + ".vcxproj"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir, projectName,projectName + ".sln"));
					break;
				case "VS2013":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2013.sln"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2013.vcxproj"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir, projectName,projectName + ".vcxproj"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir, projectName,projectName + ".sln"));
					break;
				case "VS2015":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2015.sln"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2015.vcxproj"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir, projectName,projectName + ".vcxproj"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir,projectName ,projectName + ".sln"));
					break;
				case "VS2017":
					slnFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2017.sln"));
					projFileNameSrc = new FileInfo(Path.Combine(templateDir, "VS2017.vcxproj"));
					projFileNameDest = new FileInfo(Path.Combine(projectDir,projectName,projectName + ".vcxproj"));
					slnFileNameDest = new FileInfo(Path.Combine(projectDir, projectName,projectName + ".sln"));
					break;
				default:
					MessageBox.Show(this,"�����VS�汾");
					return;
			}

			string slnContent=File.ReadAllText(slnFileNameSrc.FullName);
			//����sln�ļ�
			slnContent = slnContent.Replace("${ProjectName}", projectName);
			WriteToFile(slnContent, slnFileNameDest);

			string projContent=File.ReadAllText(projFileNameSrc.FullName);
			//����vcproj�ļ�
			projContent= projContent.Replace("${ProjectName}", projectName);
			//VS2015��win7��û���⣬��Ϊ��win7��vs2015��Ҳ������win8.1 sdk�ġ�����д<WindowsTargetPlatformVersion>8.1</WindowsTargetPlatformVersion>û����
			//vs2017�е�vcxproj���ڱ���ָ��ʹ�õ�Windows SDK��С�汾�ţ�SB΢�������Ա���ȥע����ѯ��ǰ�����а�װ��winsdk�ľ���汾
			if (ideVer== "VS2017")
			{
				//ע����64λϵͳ�£����ע�������HKLM\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots
				//������ǰ������32λ������˷���HKLM\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots��ʱ��windows���á�ע����ض��򡱻����ض���HKLM\SOFTWARE\WOW6432Node\Microsoft\\Windows Kits\\Installed Roots
				//�������ǵĳ�����Ҫ����32λ��64λϵͳ������
				var regWinSDK = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots");
				if (regWinSDK !=null&& regWinSDK.SubKeyCount>0)
				{
					//Installed Roots�������ɸ�sdk�İ汾�ţ�����ȡ��һ������
					var firstName = regWinSDK.GetValueNames().First();
					string? sdkVer = (string?)regWinSDK.GetValue(firstName);
					if (sdkVer != null)
					{
						projContent.Replace("${WindowsTargetPlatformVersion}", sdkVer);
					}
				}
			}
			WriteToFile(projContent, projFileNameDest);

			//����main.c
			File.Copy(Path.Combine(templateDir,"main.c"), Path.Combine(projectDir, projectName,"main.c"));
			CopyDir(templateDir + "/depends", projectDir + "/" + projectName + "/depends");
			CopyDir(templateDir + "/dlls", projectDir + "/" + projectName + "/dlls");

			//����/Images��Ŀ¼
			Directory.CreateDirectory(Path.Combine(projectDir,projectName, "Images"));
			Directory.CreateDirectory(Path.Combine(projectDir, projectName, "Sounds"));
			Directory.CreateDirectory(Path.Combine(projectDir, projectName, "Sprites"));

			Shell32.ShellExecute(this.Handle,"open",Path.Combine(projectDir,projectName),null,null,ShowWindowCommand.SW_NORMAL);//����Դ�������д�
			MessageBox.Show(this,"���ɳɹ�");
			Settings.Default.LastIDEVersion = (string)cmbIDEVersion.SelectedItem;
			Settings.Default.Save();
		}

        private void FormMain_Load(object sender, EventArgs e)
        {
			string lastIDEVer = Settings.Default.LastIDEVersion;
			if(!string.IsNullOrWhiteSpace(lastIDEVer))
            {
				cmbIDEVersion.SelectedItem = lastIDEVer;
			}
		}
    }
}