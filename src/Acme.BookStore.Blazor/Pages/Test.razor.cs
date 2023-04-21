using Acme.BookStore.Authors;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acme.BookStore.Blazor.Pages
{
    public partial class Test
    {
        public bool AdvancedSearch { get; set; }
        public bool IsDataGridFilterable { get; set; } = false;
        private int? Sequence { get; set; }
        private string FilterValue { get; set; }
        private AuthorDto selectedAuthorDto;
        private AuthorSearchDto AuthorSearch { get; set; } = new AuthorSearchDto();
        private List<AuthorDto> AuthorList { get; set; }
        private int PageSize { get; set; } = 10;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private AuthorDto AuthorDto { get; set; }
        public Test()
        {
            AuthorDto = new AuthorDto();
        }
        protected override async Task OnInitializedAsync()
        {
            await GetAuthorsAsync();
        }
        private async Task GetAuthorsAsync()
        {
            try
            {
                GetAuthorListDto getAuthorListDto = new GetAuthorListDto();
                getAuthorListDto.MaxResultCount = PageSize;
                getAuthorListDto.SkipCount = CurrentPage * PageSize;
                getAuthorListDto.Sorting = CurrentSorting;
                getAuthorListDto.Filter = !string.IsNullOrEmpty(FilterValue) ? FilterValue : string.Empty;
                getAuthorListDto.AuthorSearch = AuthorSearch;
                var result = await _authorAppService.GetPagedListAsync(getAuthorListDto);
                AuthorList = (List<AuthorDto>)result.Items;
                TotalCount = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AuthorDto> e)
        {
            try
            {
                var name = e.Columns.FirstOrDefault(i => i.Field == "Name" && i.SearchValue != null);
                var shortBio = e.Columns.FirstOrDefault(i => i.Field == "ShortBio" && i.SearchValue != null);
                if (shortBio is not null || name is not null )
                    AuthorSearch = new AuthorSearchDto();  
                if (name != null)
                    AuthorSearch.Name = name.SearchValue.ToString();
                if (shortBio != null)
                    AuthorSearch.ShortBio = shortBio.SearchValue.ToString();
               
                if ( name is null && shortBio is null )
                    AuthorSearch = null;
                CurrentSorting = e.Columns
                    .Where(c => c.SortDirection != SortDirection.Default)
                    .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                    .JoinAsString(",");
                CurrentPage = e.Page - 1;
                PageSize = e.PageSize;
                await GetAuthorsAsync();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task OnTableSearchButton(KeyboardEventArgs e) => await GetAuthorsAsync();
        private async Task OnClearSearchButton(CommandContext<AuthorDto> context)
        {
            Sequence = null;
            FilterValue = null;
            await context.Clicked.InvokeAsync();
        }
        private async Task OnHandelAdvancedSearch()
        {
            if (AdvancedSearch == true)
            {
                IsDataGridFilterable = false;
            }
            else
            {
                IsDataGridFilterable = true;
                AuthorSearch = null;
                await GetAuthorsAsync();
            }
        }
    }
}
