using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
namespace StandardIO
{
    class Program
    {
        static void Main(string[] args)
        {

            const int NumberOfFiles = 9;
            TxtFile[] Library = new TxtFile[NumberOfFiles];
            TxtFile File;

            File = new TxtFile("Это современная автоматическая винтовка с низким отдачей и высокой точностью.", "M4A4");
            Library[0] = File;
            File = new TxtFile("Одна из самых мощных винтовок в игре с большим уроном и скорострельностью.", "AK-47");
            Library[1] = File;
            File = new TxtFile("Это самая мощная снайперская винтовка в игре с большим уроном и малой скорострельностью.", "AWP");
            Library[2] = File;
            File = new TxtFile("Быстрый и легкий снайперский арбалет, который часто используется для дистанционной стрельбы зонтичного типа с дальней дистанции.", "SSG-533");
            Library[3] = File;
            File = new TxtFile("французский автомат с боковым расположением магазина, который готовит боеприпасы для стрельбы высокой скоростью.", "FAMAS");
            Library[4] = File;
            File = new TxtFile("Полуавтоматический пистолет, который имеет меньший урон, чем другие пистолеты.", "P250");
            Library[5] = File;
            File = new TxtFile("Невероятно точная с высоким пробитием брони, но долго перезаряжается.", "AWP");
            Library[6] = File;
            File = new TxtFile("Широко используемый автомат среднего калибра. Сильный отдача и высокая точность стрельбы в дальности.", "AK-47");
            Library[7] = File;
            File = new TxtFile("Кратковременное оружие, наносящее серьезный урон. " +
                "С большим количеством патронов и быстрой скорострельностью, оно идеально подходит для ближнего боя.", "P250");
            Library[8] = File;


            Console.WriteLine("Поиск по ключевым словам: ");
            string Request = Convert.ToString(Console.ReadLine());

            FileSearch filesearch = new FileSearch();
            filesearch.Search(Library, Request, NumberOfFiles);
            Console.WriteLine(filesearch.FoundFiles);

            Console.WriteLine("Выберите файл, который вы хотите отредактировать:");
            int FileNumber = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nТекст файла:");
            Caretaker ct = new Caretaker();
            Library[FileNumber].PrintText();
            ct.SaveState(Library[FileNumber]);

            Console.WriteLine("\nВведите новый текст файла: ");
            string NewText = Convert.ToString(Console.ReadLine());
            Library[FileNumber].Text = NewText;
            Console.WriteLine("\nСохранить новый текст? \n1 Да \n2 Нет");

            string SaveChoice = Convert.ToString(Console.ReadLine());
            if (SaveChoice == "1" || SaveChoice == "Да" || SaveChoice == "да")
            {
                Console.WriteLine("\nФайл сохранен. ");
                Library[FileNumber].PrintText();
            }
            else
            {
                ct.RestoreState(Library[FileNumber]);
                Console.WriteLine("\nФайл не удалось сохранить. ");
                Library[FileNumber].PrintText();
            }

            Console.ReadKey();
        }
    }
    class Memento
    {
        public string Text { get; set; }
    }
    public interface IOriginator
    {
        object GetMemento();
        void SetMemento(object memento);
    }

    [Serializable]
    public class TxtFile : IOriginator
    {
        public string Text;
        public string Tags;

        public TxtFile() { }

        public TxtFile(string Text, string Tags)
        {
            this.Text = Text;
            this.Tags = Tags;
        }

        public string BinarySerialize()
        {
            string FileName = "Данные файла";
            FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Flush();
            fs.Close();
            return FileName;
        }

        public void BinaryDeserialize(string FileName)
        {
            FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            TxtFile deserialized = (TxtFile)bf.Deserialize(fs);
            Text = deserialized.Text;
            fs.Close();
        }

        static public string XMLSerialize(TxtFile details)
        {
            string FileName = "Данные XML";
            XmlSerializer serializer = new XmlSerializer(typeof(TxtFile));
            using (TextWriter writer = new StreamWriter(FileName))
            {
                serializer.Serialize(writer, details);
            }
            return FileName;
        }

        static public TxtFile XMLDeserialize(string FileName)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(TxtFile));
            TextReader reader = new StreamReader(FileName);
            object obj = deserializer.Deserialize(reader);
            TxtFile XmlData = (TxtFile)obj;
            reader.Close();
            return XmlData;
        }

        public void PrintText()
        {
            Console.WriteLine(Text);
        }

        object IOriginator.GetMemento()
        {
            return new Memento { Text = this.Text };
        }
        void IOriginator.SetMemento(object memento)
        {
            if (memento is Memento)
            {
                var mem = memento as Memento;
                Text = mem.Text;
            }
        }
    }
    public class Caretaker
    {
        private object memento;
        public void SaveState(IOriginator originator)
        {
            memento = originator.GetMemento();
        }

        public void RestoreState(IOriginator originator)
        {
            originator.SetMemento(memento);
        }
    }
    class FileSearch
    {
        public string FoundFiles = "";
        public void Search(TxtFile[] library, string Request, int numberOfFiles)
        {
            for (int FileNumber = 0; FileNumber < numberOfFiles; ++FileNumber)
            {
                if (library[FileNumber].Tags == Request)
                {
                    FoundFiles += FileNumber + " ";
                }
            }

            if (FoundFiles == "")
            {
                Console.WriteLine("Файлы не найдены");
            }
            else
            {
                Console.WriteLine("\nИтог: ");
            }
        }
    }
}
