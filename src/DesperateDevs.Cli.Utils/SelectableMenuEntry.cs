using System;

namespace DesperateDevs.Cli.Utils
{
    public class SelectableMenuEntry : MenuEntry
    {
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    UpdateTitle();
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
            UpdateTitle();
            _onSelected = onSelected;
            Action = ToggleSelected;
        }

        void ToggleSelected() => IsSelected = !IsSelected;
        void UpdateTitle() => Title = (_isSelected ? "[x] " : "[ ] ") + _title;
    }
}
