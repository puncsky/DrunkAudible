// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using RestSharp;

namespace DrunkAudible
{
    public class WebServiceRequest : RestRequest
    {
        public WebServiceRequest (string resource, Method method)
            : base (resource, method)
        {
            AddHeader("Authorization", "Bearer jh8nCJo5vGRYRji7eT2DzGSn4wkljIJjey8OI9F5iMos0tFezFvwdrcJZtBc3B4EZOPCz" +
                "7kcWzWUhqEvYSzY7br7hvXHFxVUnUE6OwdHPUjZojy5MLtWu9UDn1G9QrS6vuIRWtOH3J8mLAtswesmhWD8tHMiVoER9gd8UByg" +
                "1wojqpzi3YNa53uPzkwDZkLxjTjgkfKJBAXYtQxNvg7NbLiMf1lCe7LWlQjCNk7VnBYgt4kYlrCLsgVMsRswJP6R");
        }
    }
}

