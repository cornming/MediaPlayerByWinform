using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomMusicByWinForm
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// 找尋所有檔案
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="typeDir"></param>
        /// <param name="fileUrls"></param>
        public static void DirSearch(string sDir, string typeDir, ref List<string> fileUrls)
        {
            try
            {
                //先找出所有目錄 
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    GetCurrentFiles(typeDir, fileUrls, d);
                    //此目錄處理完再針對每個子目錄做處理 
                    DirSearch(d, typeDir, ref fileUrls);
                }

                GetCurrentFiles(typeDir, fileUrls, sDir);
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        /// <summary>
        /// 針對目前目路的檔案做處理 
        /// </summary>
        /// <param name="typeDir"></param>
        /// <param name="fileUrls"></param>
        /// <param name="d"></param>
        private static void GetCurrentFiles(string typeDir, List<string> fileUrls, string d)
        {
            //針對目前目路的檔案做處理 
            foreach (string f in Directory.GetFiles(d, typeDir))
            {
                fileUrls.Add(f);
            }
        }
    }
}
