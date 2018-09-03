using BlackJackApp.BusinessLogic;
using BlackJackApp.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

//CLASS AUTHOR: Michael Banks
namespace BlackJackApp.DataPersistence
{
    class BlackJackTextSerializer
    {

        /// <summary>
        /// Field Variable used to store the directory path
        /// </summary>
        private string _directoryPath;

        /// <summary>
        /// Field Variable used to create a blackjack player
        /// </summary>
        private BlackJackUser _player;

        /// <summary>
        /// Field Variable used to store the path of the file
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Constructor that initializes the field variables and determines if the folder for the file already exists
        /// </summary>
        public BlackJackTextSerializer()
        {
            //initializes the player to null - it is set later
            _player = null;

            //sets the file path
            _filePath = $"{_directoryPath}/playerinfo.dat";
            
            //sets the directory path
            _directoryPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "BlackJackSaves");

            //if the directory doesn't exist
            if (Directory.Exists(_directoryPath) == false)
            {
                //create the directory
                Directory.CreateDirectory(_directoryPath);
            }
        }

        /// <summary>
        /// Property used to access the player field variable from outside of the class 
        /// </summary>
        public BlackJackUser Player
        {
            get { return _player; }
            set { _player = value; }
        }

        /// <summary>
        /// Read-Only Property used to access the file path from outside of the class
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        /// <summary>
        /// Read-Only Property used to access the directory path from outside of the class
        /// </summary>
        public string DirectoryPath
        {
            get { return _directoryPath; }
        }

        /// <summary>
        /// Method used to load the data from a file
        /// </summary>
        public void Load()
        {
            //access the files from the directory - one in this case
            string[] directoryPath = Directory.GetFiles(_directoryPath);

            //loop through each file in the directory - one in this case
            foreach (string file in directoryPath)
            {
                //open the file and load the content from it, close it afterwards
                using (StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open)))
                {
                    _player.Load(reader);
                }
            }
        }

        /// <summary>
        /// Method used to save the data to a file
        /// </summary>
        public void Save()
        {
            //stores the path of the file in a variable
            string filePath = $"{_directoryPath}/playerinfo.dat";

            //open the file and write to it
            using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                _player.Save(writer);
            }

        }


    }
}
