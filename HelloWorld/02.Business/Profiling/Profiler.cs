using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using System.Diagnostics;

namespace WindowsFormsApplication7.Business.Profiling
{
    class Profiler
    {
        public Dictionary<string, ProfileSection> allSections = new Dictionary<string, ProfileSection>();
        public static Profiler Instance = new Profiler();
        public ProfileSection root;
        public ProfileSection activeSection;
        public string SelectedSection = "root";
        public string MarkedSection = "root";
        public bool Enabled = true;
        public bool ReportCounters = false;

        public Profiler()
        {
            root = new ProfileSection("root");
            allSections.Add(root.Name, root);
            Clear();
        }

        public class ProfileSection
        {
            public string Name;
            public Stopwatch Stopwatch = new Stopwatch();
            public ProfileSection Parent = null;
            public List<ProfileSection> Children = new List<ProfileSection>();
            public ProfileSection(string name)
            {
                Name = name;
            }
        }

        internal void StartSection(string sectionName)
        {
            if (!Enabled) return;
            sectionName = activeSection.Name + "." + sectionName.ToLower();
            ProfileSection child = activeSection.Children.Where(c => c.Name == sectionName).FirstOrDefault();
            if (child == null)
            {
                child = new ProfileSection(sectionName);
                allSections.Add(child.Name, child);
                activeSection.Children.Add(child);
                child.Parent = activeSection;

            }
            activeSection = child;
            child.Stopwatch.Start();
        }

        internal void EndSection()
        {
            if (!Enabled) return;
            activeSection.Stopwatch.Stop();
            activeSection = activeSection.Parent;
        }

        internal void EndStartSection(string sectionName)
        {
            if (!Enabled) return;
            sectionName = sectionName.ToLower();
            EndSection();
            StartSection(sectionName);
        }

        public void ToggleMarkedSection()
        {
            if (!Enabled) return;
            ProfileSection profileSection = allSections[SelectedSection];
            ProfileSection markedSection = allSections[MarkedSection];
            if (profileSection == markedSection)
                MarkedSection = profileSection.Children[0].Name;
            else
            {
                int newIndex = profileSection.Children.IndexOf(markedSection);
                if (newIndex == profileSection.Children.Count - 1)
                    MarkedSection = SelectedSection;
                else
                    MarkedSection = profileSection.Children[newIndex + 1].Name;
            }



        }



        internal string Report()
        {
            StringBuilder sb = new StringBuilder();

            int padding = 50;
            if (ReportCounters)
            {
                foreach (var pair in Counters.Instance.AllCounters)
                {
                    sb.AppendLine(string.Format("{0} = {1}", pair.Key.PadRight(padding), pair.Value));

                }
                return sb.ToString(); ;
            }
            else
            {

                ProfileSection profileSection = allSections[SelectedSection];
                ProfileSection markedSection = allSections[MarkedSection];
                string line = string.Format("{0} : {1:000.}% ({2}ms)", ((profileSection == markedSection ? "=> " : "") + profileSection.Name).PadRight(padding), profileSection.Stopwatch.ElapsedMilliseconds * 100f / root.Stopwatch.ElapsedMilliseconds, profileSection.Stopwatch.ElapsedMilliseconds);
                sb.AppendLine(line);
                foreach (var child in profileSection.Children)
                {
                    line = string.Format("{0} : {1:000.}% ({2}ms)", ((child == markedSection ? "=> " : "") + child.Name).PadRight(padding), child.Stopwatch.ElapsedMilliseconds * 100f / profileSection.Stopwatch.ElapsedMilliseconds, child.Stopwatch.ElapsedMilliseconds);
                    sb.AppendLine(line);
                }
                return sb.ToString();
            }
        }

        internal void Clear()
        {
            if (!Enabled) return;

            activeSection = root;
            allSections.Values.ToList().ForEach(s => s.Stopwatch.Reset());
            root.Stopwatch.Start();
            Counters.Instance.Reset();
        }



        internal void ToggleEnabled()
        {
            Enabled = !Enabled;
        }

        internal void ToggleReport()
        {
            ReportCounters = !ReportCounters;
        }
    }
}