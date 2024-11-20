using System.Runtime.Serialization;

namespace Tourism.Enums
{
    public class Enums
    {
        public enum Cities
        {
 
            Tehran = 1,
 
            Mashhad = 2,

            Isfahan = 3,

            Shiraz = 4,

            Tabriz = 5
        }
        public enum ArticleTopic
        {
          
            Historical = 1,

            Nature = 2,
  
            Entertainment = 3,
 
            Religious = 4,

 
            Culinary = 6,

            Art = 7,

            TravelGuide = 8,

            Neighborhood = 9,

            Festivals = 10
        }
        public enum TicketStatus
        {
            Open,             
            WaitingForResponse=1, 
            Responded=2,         
            Closed =3            
        }
    }
}
        