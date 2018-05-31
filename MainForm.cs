using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace MyEBookReader
{
    public partial class MainForm : Form
    {
        private string theEBook = "";
        string[] tenMostCommon = new string[10];
        string longestWord = string.Empty;
//------------------------------------------------------------------------------------------------------------------------------------------------
        public MainForm()
        {
            InitializeComponent();
        }
//------------------------------------------------------------------------------------------------------------------------------------------------
        private void btnDownload_Click(object sender, EventArgs e)
        {
            // The Project Gutenberg EBook of A Tale of Two Cities, by Charles Dickens
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (s, eArgs) => 
            { 
                theEBook = eArgs.Result;
                txtBook.Text = theEBook;
            };
            wc.DownloadStringAsync(new Uri("http://www.gutenberg.org/files/98/98-8.txt"));            
        }
//----------------------------------------------------------------------------------------------------------------------------------------------------
        #region Do work in parallel!
        private void btnGetStats_Click(object sender, EventArgs e)
        {

            //string[] words = null; //TODO: Получить слова из книги theBook в виде массива
            string[] words = theEBook.Split(new Char[] {' ' , ',' , '.' ,':' , '\t' ,'\r' ,'[' , ']' ,'\n' , '-','/','?','"'});
            
           //TODO: Запустить параллельно задачи Parallel.Invoke
            //FindTenMostCommon и FindLongestWord
            try
            {
                Parallel.Invoke(
                () =>
                {
                      tenMostCommon = FindTenMostCommon(words);
                },
                () =>
                {
                        longestWord = FindLongestWord(words);
                }
                );

            }
            catch (Exception )
            {
                throw new NotSupportedException();
            }


            //...
//-------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Сейчас все задачи уже выполнены,
            // формируем строку для вывода в MessageBox
            StringBuilder bookStats = new StringBuilder("Ten Most Common Words are:\n");
            foreach (string s in tenMostCommon)
            {
                bookStats.AppendLine(s);
            }
            bookStats.AppendFormat("The longest word is: {0}", longestWord);
            bookStats.AppendLine();
            MessageBox.Show(bookStats.ToString(), "Book info");
        }
        #endregion

        #region Task methods.
  //--------------------------------------------------------------------------------------------------------------------------------------------
        private string[] FindTenMostCommon(string[] words)
        {
            //TODO:
            //var frequencyOrder = ...;
            //Используя LINQ найти 10 наиболее часто встречаемых слов
            string[] commonWords = new string[10];
       
               var wordsToGroup = words.GroupBy(x => x.ToLower()).OrderByDescending(x=>x.Count()).Take(10);
         
            var j = 0;
            foreach (var i in wordsToGroup)
            {
                commonWords[j] = i.Key.ToString();
                j++;
            }
          
            return commonWords;
        }
//----------------------------------------------------------------------------------------------------------------------------------------
        private string FindLongestWord(string[] words)
        {
            //TODO: Найти самое длинное слово
            //также можно использовать LINQ
            int wordMaxLength = words.Select(x => x.Length).Where(x => x > 0).Select(x => x).Max();
            string longestWord = words.Where(x => x.Length == wordMaxLength).Select(x => x).First();
            return longestWord;
        }
        #endregion
    }
}
