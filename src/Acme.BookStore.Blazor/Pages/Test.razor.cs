using Acme.BookStore.Authors;
using Acme.BookStore.Books;
using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acme.BookStore.Blazor.Pages
{
    public partial class Test
    {

        public SexStatusEnum? SexStatus { get; set; }
        public List<String> SearchBookList { get; set; }
        public DateTime? BirthDate { get; set; }

        public bool AdvancedSearch { get; set; }
        public bool IsDataGridFilterable { get; set; } = false;
        private int? Sequence { get; set; }
        private string FilterValue { get; set; }
        private BookDto selectedBookDto;
        private AuthorSearchDto AuthorSearch { get; set; } = new AuthorSearchDto();
        private List<BookDto> AuthorBookList { get; set; }
        private int PageSize { get; set; } = 10;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }
        private BookDto BookDto { get; set; }
        public Test()
        {
            BookDto = new BookDto();
        }
        protected override async Task OnInitializedAsync()
        {
            await GetAuthorsAsync();
        }
        private async Task GetAuthorsAsync()
        {
            try
            {
                GetAuthorListDto dto = new GetAuthorListDto();
                dto.MaxResultCount = PageSize;
                dto.SkipCount = CurrentPage * PageSize;
                dto.Sorting = CurrentSorting;
                dto.Filter = !string.IsNullOrEmpty(FilterValue) ? FilterValue : string.Empty;
                dto.AuthorSearch = AuthorSearch;
                var result = await _bookAppService.GetAuthorsbookListAsync(dto);
                AuthorBookList = (List<BookDto>)result.Items;
                TotalCount = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<BookDto> e)
        {
            try
            {
                var authorName = e.Columns.FirstOrDefault(i => i.Field == "AuthorName" && i.SearchValue != null);
                var bookName = e.Columns.FirstOrDefault(i => i.Field == "Name" && i.SearchValue != null);
                var shortBio = e.Columns.FirstOrDefault(i => i.Field == "ShortBio" && i.SearchValue != null);
                var sex = e.Columns.FirstOrDefault(i => i.Field == "Sex" && i.SearchValue != null);
                var birthDate = e.Columns.FirstOrDefault(i => i.Field == "Birthdate" && i.SearchValue != null);
                var price = e.Columns.FirstOrDefault(i => i.Field == "Price" && i.SearchValue != null);
                if (shortBio is not null || authorName is not null || sex is not null || birthDate is not null || price is not null)
                    AuthorSearch = new AuthorSearchDto();  
                if (authorName != null)
                    AuthorSearch.AuthorName = authorName.SearchValue.ToString();
                
                if (sex != null && sex.SearchValue.ToString() !="All")
                    AuthorSearch.Sex = sex.SearchValue.ToString();
                //if (birthDate != null )
                //    AuthorSearch.Birthdate = (DateTime)birthDate.SearchValue;
                if (bookName != null)
                    AuthorSearch.BookName = bookName.SearchValue.ToString();
                if (price != null)
                    AuthorSearch.Price = Single.TryParse(price.SearchValue.ToString(), out float value) ? value : 0.0f;

                if (authorName is null &&  sex is null && birthDate is null && bookName is null && price is null)
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
        private async Task OnClearSearchButton(CommandContext<BookDto> context)
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
                SearchBookList= await _bookAppService.GetBookList();
            }
        }
    }
}
