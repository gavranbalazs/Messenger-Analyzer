using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Helpers
{
    public class ProjectDescription
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Version { get;} = "V1.0";

        public string Creator { get; set; }

        public DateTime CreationDate { get; set; }

        public string[] RawFiles { get; set; }

        public string MergedFile { get; set; }

        public string ProjectFolderLocation { get; set; }

        public Dictionary<string, string> StatisticsFile { get; set; }

        ProjectConfig ProjectConfig = new ProjectConfig();


        public bool WriteToJson(string filePath)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(this);
                System.IO.File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to JSON: {ex.Message}");
                return false;
            }
        }

        //empty constructor
        public ProjectDescription()
        {
            
        }

        //loader constructor
        public ProjectDescription(string filePath)
        {
            try
            {
                ProjectDescription projectDescription = new ProjectDescription();
                string json = System.IO.File.ReadAllText(filePath);
                projectDescription = System.Text.Json.JsonSerializer.Deserialize<ProjectDescription>(json);
                this.Name = projectDescription.Name;
                this.Description = projectDescription.Description;
                this.Version = projectDescription.Version;
                this.Creator = projectDescription.Creator;
                this.CreationDate = projectDescription.CreationDate;
                this.RawFiles = projectDescription.RawFiles;
                this.MergedFile = projectDescription.MergedFile;
                this.StatisticsFile = projectDescription.StatisticsFile;
                this.ProjectFolderLocation = projectDescription.ProjectFolderLocation;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading from JSON: {ex.Message}");

            }
        }

        //constructor with parameters
        public ProjectDescription(string name, string description, string creator, DateTime creationDate, string[] rawFiles, string mergedFile, Dictionary<string, string> statisticsFile, string projectFolderLocation)
        {
            Name = name;
            Description = description;
            Creator = creator;
            CreationDate = creationDate;
            RawFiles = rawFiles;
            MergedFile = mergedFile;
            StatisticsFile = statisticsFile;
            ProjectFolderLocation = projectFolderLocation;
        }
    }
}
