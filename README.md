# UnityDotsSpaceShooter

The project uses unity dots. Everything is divided into responsibility groups, there is one system handling player movement, one for enemy movement.

the systems handle data instead of objects. 'EnemyMovementSystem.cs' handles movement for everything with the 'EnemyTag', 'OutOfBounds.cs' checks which entities is outside the allowed area and add the tag 'DeleteTag' to them. 

Resources used:
https://www.youtube.com/watch?v=ILfUuBLfzGI&list=PLzDRvYVwl53s40yP5RQXitbT--IRcHqba
https://www.youtube.com/watch?v=KwwHvH3GZsQ
