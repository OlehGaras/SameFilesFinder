using System.Collections.Generic;
using SameFileFinder;

namespace WpfSameFileFinder
{
    public class GroupViewModel: ViewModelBase
    {
        public FileGroup Group;

        public GroupViewModel(FileGroup gr)
        {
            Group = gr;
        }

        public List<FileInfo> Files
        {
            get
            {
                return Group.Files;
            }

            set
            {
                OnPropertyChanged("Files");
            }
        }
        
    }
}
