using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace WpfApp7;

public class UserService
{
    private readonly AppContext _db;
    private readonly IMemoryCache _cache;
    
    public UserService(AppContext context, IMemoryCache memoryCache)
    {
        _db = context;
        _cache = memoryCache;
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        var result = _cache.TryGetValue(id, out User? user);
        
        if (result) return user;
        
        user = await _db.Users.FirstOrDefaultAsync(p => p.Id == id);
        if (user != null)
        {
            _cache.Set(user.Id, user, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(0.5)));
        }
        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }
    
    public void AddUserToCash(User user)
    {
        _cache.Set(
            key:user.Id,
            value:user,
            options: new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(0.5)));
    }
}