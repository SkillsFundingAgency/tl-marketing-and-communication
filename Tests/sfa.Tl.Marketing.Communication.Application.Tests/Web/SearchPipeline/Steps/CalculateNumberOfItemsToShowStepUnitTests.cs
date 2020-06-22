using FluentAssertions;
using sfa.Tl.Marketing.Communication.Constants;
using sfa.Tl.Marketing.Communication.Models;
using sfa.Tl.Marketing.Communication.SearchPipeline;
using sfa.Tl.Marketing.Communication.SearchPipeline.Steps;
using System.Threading.Tasks;
using Xunit;

namespace sfa.Tl.Marketing.Communication.UnitTests.Web.SearchPipeline.Steps
{
    public class CalculateNumberOfItemsToShowStepUnitTests
    {
        private readonly ISearchStep _searchStep;
        private readonly ISearchContext _context;

        public CalculateNumberOfItemsToShowStepUnitTests()
        {
            _context = new SearchContext(new FindViewModel());
            _searchStep = new CalculateNumberOfItemsToShowStep();
        }

        [Fact]
        public async Task Step_Initialize_NumberOfItems_To_Default_When_SubmitType_Is_Search()
        {
            // Arrange
            InitializeViewModel();
            _context.ViewModel.SubmitType = "search";

            // Act
            await _searchStep.Execute(_context);

            // Assert
            _context.ViewModel.NumberOfItemsToShow.Should().Be(AppConstants.DefaultNumberOfItemsToShow);
            _context.ViewModel.SelectedItemIndex.Should().Be(0);
            _context.ViewModel.TotalRecordCount.Should().Be(0);
        }

        [Fact]
        public async Task Step_Initialize_NumberOfItems_To_Default_When_A_Different_Qualification_Is_Selected()
        {
            // Arrange
            InitializeViewModel();
            _context.ViewModel.SearchedQualificationId = 9;
            _context.ViewModel.SelectedQualificationId = 1;

            // Act
            await _searchStep.Execute(_context);

            // Assert
            _context.ViewModel.NumberOfItemsToShow.Should().Be(AppConstants.DefaultNumberOfItemsToShow);
            _context.ViewModel.SelectedItemIndex.Should().Be(0);
        }

        [Fact]
        public async Task Step_Initialize_NumberOfItems_To_Default_When_NumberOfItems_Has_No_Value()
        {
            // Arrange
            InitializeViewModel();
            _context.ViewModel.NumberOfItemsToShow = null;

            // Act
            await _searchStep.Execute(_context);

            // Assert
            _context.ViewModel.NumberOfItemsToShow.Should().Be(AppConstants.DefaultNumberOfItemsToShow);
        }

        [Fact]
        public async Task Step_Initialize_SelectedItemIndex_To_Default_When_TotalRecordCount_Has_No_Value()
        {
            // Arrange
            InitializeViewModel();
            _context.ViewModel.TotalRecordCount = null;

            // Act
            await _searchStep.Execute(_context);

            // Assert
            _context.ViewModel.SelectedItemIndex.Should().Be(0);
        }

        [Fact]
        public async Task Step_Increment_NumberOfItemsToShow_And_Set_SelectedItemIndex_When_User_Click_On_The_ShowMore_Button()
        {
            // Arrange
            InitializeViewModel();
            int currentNumberOfItemsToShow = 10;
            int totalRecordCount = 20;
            int qualificationId = 9;
            _context.ViewModel.TotalRecordCount = totalRecordCount;
            _context.ViewModel.NumberOfItemsToShow = currentNumberOfItemsToShow;
            _context.ViewModel.SearchedQualificationId = qualificationId;
            _context.ViewModel.SelectedQualificationId = qualificationId;

            // Act
            await _searchStep.Execute(_context);

            // Assert
            _context.ViewModel.SelectedItemIndex.Should().Be(currentNumberOfItemsToShow);
            _context.ViewModel.NumberOfItemsToShow.Should().Be(currentNumberOfItemsToShow + AppConstants.DefaultNumberOfItemsToShow);
        }

        [Fact]
        public async Task Step_Does_Not_Increment_NumberOfItemsToShow_And_Set_SelectedItemIndex_When_Remaining_Items_Are_Less_Than_Default()
        {
            // Arrange
            InitializeViewModel();
            int currentNumberOfItemsToShow = 10;
            int totalRecordCount = 13;
            int qualificationId = 9;
            _context.ViewModel.TotalRecordCount = totalRecordCount;
            _context.ViewModel.NumberOfItemsToShow = currentNumberOfItemsToShow;
            _context.ViewModel.SearchedQualificationId = qualificationId;
            _context.ViewModel.SelectedQualificationId = qualificationId;

            // Act
            await _searchStep.Execute(_context);

            // Assert
            _context.ViewModel.SelectedItemIndex.Should().Be(currentNumberOfItemsToShow);
            _context.ViewModel.NumberOfItemsToShow.Should().Be(totalRecordCount);
        }

        private void InitializeViewModel()
        {
            _context.ViewModel.TotalRecordCount = null;
            _context.ViewModel.NumberOfItemsToShow = null;
            _context.ViewModel.SearchedQualificationId = 0;
            _context.ViewModel.SelectedQualificationId = null;
            _context.ViewModel.SubmitType = string.Empty;
        }
    }
}
