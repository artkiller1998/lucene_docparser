using Lucene.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lucene;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using System.Windows.Forms;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis;

namespace luceneStart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class IndexBuilder
        {
            String indexDir;
            String outDir;
            String stemDir;
            String standDir;

            IndexWriter writer;
            IndexWriter stem_writter;

            public IndexBuilder (string indDir, string oDir)
            {
                indexDir = indDir;
                outDir = oDir;
                stemDir = Path.Combine(outDir, "stemming");
                standDir = Path.Combine(outDir, "standard");
                //Lucene.Net.Store.Directory dir = FSDirectory.Open(outDir);


                Analyzer snowball = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_30, "English");
                //AnalyzerUtils.assertAnalyzesTo(snowball, "stemming algorithms",new String[] { "stem", "algorithm" });
                
                stem_writter = new IndexWriter(
                FSDirectory.Open(stemDir), snowball,
                true, IndexWriter.MaxFieldLength.UNLIMITED);


                Index(indDir,stem_writter);

                writer = new IndexWriter(
                FSDirectory.Open(standDir),
                new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                true, IndexWriter.MaxFieldLength.UNLIMITED);

                Index(indDir,writer);
                

                CloseAll();
            }

            public void CloseAll()
            {
                writer.Close();
                stem_writter.Close();
            }
            public int Index(System.String dataDir, IndexWriter writer)
            {
                System.String[] files = System.IO.Directory.GetFileSystemEntries(dataDir);
                for (int i = 0; i < files.Length; i++)
                {
                    IndexFile(new System.IO.FileInfo(files[i]),writer);
                }
                return writer.NumDocs();
            }
           
            protected Document GetDocument(System.IO.FileInfo file)
            {
                Document doc = new Document();
                doc.Add(new Field("contents",
                new System.IO.StreamReader(file.FullName,
                System.Text.Encoding.Default)));
                doc.Add(new Field("filename",
                file.Name,
                Field.Store.YES,
                Field.Index.NOT_ANALYZED));
                doc.Add(new Field("fullpath",
                file.FullName,
                Field.Store.YES,
                Field.Index.NOT_ANALYZED));
                return doc;
            }
            private void IndexFile(System.IO.FileInfo file, IndexWriter writer)
            {
                Document doc = GetDocument(file);
                writer.AddDocument(doc);
            }
        }

        class Searcher
        {
            String indexDir;
            String q ;

            public Searcher(string indDir, string qu)
            {
                indexDir = indDir;
                q = qu;
                search(indexDir, q);
            }
            public static void search(String indexDir, String q)
            {
                Lucene.Net.Store.Directory dir = FSDirectory.Open(indexDir);
                IndexSearcher searcher = new IndexSearcher(dir);
                QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30,"contents",new SimpleAnalyzer());
                Query query = parser.Parse(q);
                Lucene.Net.Search.TopDocs hits = searcher.Search(query,null, 10, Sort.RELEVANCE);
                MessageBox.Show("Found " + hits.TotalHits + " document(s) that matched query '" + q + "':");
                for (int i = 0; i < hits.ScoreDocs.Length; i++)
                {
                    ScoreDoc scoreDoc = hits.ScoreDocs[i];
                    Document doc = searcher.Doc(scoreDoc.Doc);
                    MessageBox.Show(doc.Get("filename"));
                }
                searcher.Close();
                dir.Close();
            }
        }

        IndexBuilder obj = new IndexBuilder(@"C:\Users\User\Desktop\books — копия\docParser", @"C:\Users\User\Desktop\index");
        //Searcher search = new Searcher(@"C:\Users\User\Desktop\index\stemming", "Err");
    }
}








    //    public Form1()
    //    {
    //        InitializeComponent();
    //        //args[0] = @"D:/";
    //        //args[1] = @"D:/";
    //        //Указываем директорию для хранения индекса 
    //        Lucene.Net.Store.Directory dir = FSDirectory.Open(@"C:\index");
    //        //Определяем морфологический анализатор
    //        Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT);
    //    }

//    static Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT);

//    IndexWriter writer = new IndexWriter(directoryIndex, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
//    private static Lucene.Net.Store.Directory directoryIndex;


//    string[] files = System.IO.Directory.GetFiles(@"C:\index");

//    foreach (string file in files)
//    {
//     string text = File.ReadAllText(file);
//    Document doc = new Document();
//    string name = System.IO.Path.GetFileName(file);
//    doc.Add(new Field("name", name, Field.Store.YES, Field.Index.ANALYZED));
//     doc.Add(new Field("text", text, Field.Store.YES, Field.Index.ANALYZED));
//     writer.AddDocument(doc);
//    }
//writer.Close();

//    public void Indexing()
//    {
//    //Определяем записывающего в индекс
//    IndexWriter writer = new IndexWriter(dir, analyzer, MaxFieldLength.UNLIMITED);
//    //Получаем файлы подлежащие индексированию
//    string[] files = Directory.GetFiles(folderBrowser.SelectedPath);
//    //Для каждого файла получаем его текст и имя
//    foreach (String file in files)
//    {
//        string text = System.IO.File.ReadAllText(file);
//        string name = System.IO.Path.GetFileName(file);

//        //Определяем документ
//        Document doc = new Document();
//        //Добавляем поля в документ
//        doc.Add(new Field("name", name, Field.Store.YES, Field.Index.NOT_ANALYZED));
//        doc.Add(new Field("text", text, Field.Store.NO, Field.Index.ANALYZED));

//        //Записываем документы в индекс
//        writer.AddDocument(doc);
//    }
//    writer.Dispose();
//    ResultsTextBox.Text = "Индексирование завершено!";
//    }


















//Analyzer analyzer = new StandardAnalyzer();
//IndexWriter writer = new IndexWriter("Test", analyzer);

//for (int i = 0; i< 1000000; i++)
//{
//    string id = i.ToString();
//    string text = string.Format("{0} string.", i);

//    Document doc = new Document();

//public IndexWriter Writer { get => writer; set => writer = value; }

//doc.Add(new Field("id", id,
//    Field.Store.YES, Field.Index.NO));

//    doc.Add(new Field("text", text,
//    Field.Store.YES, Field.Index.TOKENIZED));

//    writer.AddDocument(doc);

//    if (i % 100000 == 0)
//    Console.WriteLine("[{1}]: {0} documents are
//    saved.", i, DateTime.Now);
//}

//writer.Close();


