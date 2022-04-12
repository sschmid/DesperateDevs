using System;

namespace DesperateDevs.Cli.Utils
{
    public class SelectableMenuEntry : MenuEntry
    {
        public bool isSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    updateTitle();
                    _onSelected?.Invoke(_isSelected);
                }
            }
        }

        readonly string _title;
        bool _isSelected;
        readonly Action<bool> _onSelected;

        public SelectableMenuEntry(string title, bool isSelected, Action<bool> onSelected = null) :
            base(title, null, false, null)
        {
            _title = title;
            _isSelected = isSelected;
            updateTitle();
            _onSelected = onSelected;
            action = toggleSelected;
        }

        void toggleSelected()
        {
            isSelected = !isSelected;
            updateTitle();
        }

        void updateTitle()
        {
            title = (_isSelected ? "[x] " : "[ ] ") + _title;
        }
    }
}
