using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WpfApp7;

public class MainWindowModel : BaseWindowModel
{
    private readonly UserService _userService;
    
    private int _id;
    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }
    
    private string _name;
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }
    
    private int _age;
    public int Age
    {
        get => _age; 
        set => SetField(ref _age, value);
    }
    
    public ObservableCollection<User> Users {get; set; }
    
    private User _selectedUser;
    public User SelectedUser
    {
        get => _selectedUser;
        set
        {
            var result = SetField(ref _selectedUser, value);
            if (!result) return;
            
            Id = _selectedUser.Id;
            Name = _selectedUser.Name;
            Age = _selectedUser.Age;
                
            _userService.AddUserToCash(_selectedUser);
        }
    }
    
    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set => SetField(ref _searchText, value);
    }
    
    public LambdaCommand SearchCommand { get; }

    public MainWindowModel()
    {
        var options = new DbContextOptionsBuilder<AppContext>()
            .UseSqlite("Data Source=test.db")
            .Options;
        var db = new AppContext(options);
        
        _userService = new UserService(db, new MemoryCache(new MemoryCacheOptions()));
        
        Users = new ObservableCollection<User>(_userService.GetAllUsersAsync().Result);
        
        SearchCommand = new LambdaCommand(
            execute: async (_) =>
            {
                var result = int.TryParse(SearchText, out var id);
                if (!result)
                {
                    MessageBox.Show("Неверный формат");
                    return;
                }
                
                var user = await _userService.GetUserByIdAsync(id);
                if (user is null)
                {
                    MessageBox.Show("Не найден пользователь");
                    return;
                }
                
                SelectedUser = user;
            });
    }
}