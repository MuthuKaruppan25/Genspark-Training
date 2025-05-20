using System;

class Posts
{
    public string? caption { get; set; }
    public int likes { get; set; }

}
class Program
{
    
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the no of users: ");
        int no_of_users = getUsersInput();
        Posts[][] posts = new Posts[no_of_users][];
        getPosts(posts, no_of_users);
        Console.WriteLine("The posts are: ");
        DisplayPosts(posts);

    }
    static void getPosts(Posts[][] posts, int no_of_users)
    {
        for(int i=0;i<no_of_users;i++)
        {
            Console.WriteLine($"Enter the no of posts for User {i+1}");
            int no_of_posts = getUsersInput();
            Posts[] postData = new Posts[no_of_posts];
            getPostsData(postData);
            posts[i] = postData;
        }
    }
    static void getPostsData(Posts[] posts)
    {
        for(int i=0;i<posts.Length;i++)
        {
            Console.WriteLine($"Enter the caption for post {i+1}");
            string? caption = getCaption();
            Console.WriteLine($"Enter the likes for post {i+1}");
            int likes = getUsersInput();
            posts[i] = new Posts{caption = caption, likes = likes};
        }
    }
    static void DisplayPosts(Posts[][] posts)
    {
        for(int i=0;i<posts.Length;i++)
        {
            Console.WriteLine($"User {i+1}:");
            for(int j=0;j<posts[i].Length;j++)
            {
                Console.WriteLine($"Post {j+1}:");
                Console.WriteLine($"Caption: {posts[i][j].caption}");
                Console.WriteLine($"Likes: {posts[i][j].likes}");
            }
        }
    }
    static string? getCaption()
    {
        string? caption = Console.ReadLine()!.Trim();
        if (string.IsNullOrEmpty(caption))
        {
            Console.WriteLine("Invalid input. Please enter a valid caption.");
            return getCaption();
        }
        return caption;
    }
    static int getUsersInput()
    {
        int no_of_us;
        bool isvalid = int.TryParse(Console.ReadLine(), out no_of_us);
        if (!isvalid || no_of_us <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            return getUsersInput();
        }
        return no_of_us;
    }
}