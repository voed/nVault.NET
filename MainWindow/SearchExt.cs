using Syncfusion.UI.Xaml.Grid;

namespace nVault.NET
{
    public class SearchExt : SearchHelper
    {
        private Utils.SearchType _searchType;

        public SearchExt(SfDataGrid datagrid) : base(datagrid)
        {
        }

        public void SetSearch(Utils.SearchType type)
        {
            _searchType = type;
        }

        protected override bool SearchCell(DataColumnBase column, object record, bool highlight)
        {
            switch (_searchType)
            {
                case Utils.SearchType.SearchAll:
                    return base.SearchCell(column, record, highlight);

                case Utils.SearchType.SearchKey:
                case Utils.SearchType.SearchTimestamp:
                case Utils.SearchType.SearchValue:
                {
                    if(column.GridColumn.MappingName.Equals(Utils.SearchPhrases[(int)_searchType]))
                        return base.SearchCell(column, record, highlight);
                    break;
                }
                case Utils.SearchType.SearchDate:
                {
                    if (column.GridColumn.MappingName.Equals("Timestamp(raw)"))
                    {
                        return base.SearchCell(column, record, highlight);
                    }
                    break;
                }
                default:
                    return false;
            }

            return false;
        }
    }
}