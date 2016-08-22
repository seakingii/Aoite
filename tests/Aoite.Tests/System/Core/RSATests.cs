using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace System
{
    public class RSATests
    {
        private readonly ITestOutputHelper output;
        public RSATests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void CreateTest()
        {
            var rsaKey = RSA.Create();

            Assert.NotNull(rsaKey.Key);
            Assert.NotNull(rsaKey.PublicKey);
            Assert.Equal(2048, rsaKey.KeySize);
            output.WriteLine("Key");
            output.WriteLine(rsaKey.Key);
            output.WriteLine("Public Key");
            output.WriteLine(rsaKey.PublicKey);
        }

        private const string Key = "<RSAKeyValue><Modulus>yAI5/1bP0BN4y0B/tqDGl15tVDA+9pT3sb666lV3cXrNJucupJHHrlP8rFRDJ9tc3uefPstKiBDGREF/oX153u+PizEZXm5mb8b1cmNbQj33yf6y7WtkyKIftDenf64OUpj92/U8YcFYFmmvHfBIfPlRvCXA8tBTbRcsDcgC7AIKpJMvR8wAqZYx5vkQ5WgsgA7hIQ1diEXuxk6lKKD/nWiEkozZvrvAo38/DTh0nUzdKhHA6nZTT+rFoKRDpLS6JxXIvSI+roUPb4WPv4gf8CYa5MNYLq/pxmacuoidqjCIjSUMmm31duITNNmtvyUkmv/i4ptp1Wqev+uNAZtSTw==</Modulus><Exponent>AQAB</Exponent><P>8Pbrw5GqtJEQ96JdW0/Lbw3aNkb6Fxq5AnXPEs8wrLdZinUR2OmKTW+eDuLLx0oxnG6BnjFlW6kAAmijPTJP9Cv/kHktsF8eX5Ga2vwkbvyVqpKN42FdJwI9L0AxFkKeo7eodAdV5YNYsQjQGiXs8UkwOD1ZGCGQRi2lIeHCP18=</P><Q>1H0XkrIl012wGhFkvPyXfvrAHNQi0gokmmzjtTIinDuUL6xZpZUaQKk4tCSKNnehAtFZlI62JSo0WKz0OmmEySY8fufXbeZSmtC95oREiXDvDc/8mU9KOiJQ/Ch40QCEevmjpWNWwtjyn50nmk5fwKH3XgxRKaSG7f8kf6FpAxE=</Q><DP>0xCmEKb+bK2IvDBXL81kN0Fd+x8OnsBEf/grSqF7VD5ByzqiSGrAPvXe79EOh+DgNdLB7Iv96VzN6k43djokuI71i1npgEzA8Rs8Fka3rxPGESPP7vRwwOOALmw+0u3FwUf04LUwMxpqzJ8YkV7Y/byK5SgvayizFBWVSGlUWWc=</DP><DQ>AKeqDO+zoiUlx87J6rM0nFWVhgE5YkBZwIgZY5OV+7lLSaJUXGhLPmvHGvcJ3gUcX+/ZQNX3xcGNi8BsuEle+We+w/1e8p3FH2kJTM6Xj3zjn71GYAqYhflZGo3dYXeI8QAX+wiFWfBlPwyy9IzNSsC1DHspPOGe8NjaefI7aqE=</DQ><InverseQ>NCUf6ikhBROplKYb/5/IrVxWEonpxB7jRixDu6jSYxy7Y5lyIFvADZY5cr3ktG/lmrOVivr4nzTk2yB30awjj6bBjlL5vjoXIvo8g6F1fkdjk7RWE3nVt/j5TvqJUu7Fzj1kwk5nYAc5N6Y7xwxfpQ7rgnL8zFeZc9/snyTJXYU=</InverseQ><D>DO/P7osf7/aCdqsaV6a4kavi6HIicHKcPtv2XRfgHO/zexOy+aQQRV9bVY4xtc9Dh4t1ymr9zCVBncKazcg+7MOUBk3LtUbwy0QUNizlBzjY4TDfQ+oRX8TUxiRfaivI82C/8s0+9Ze66eA2GnqjRaoetku+2jXNX54DFHUec0NlNknnoJFQcs08QvPISqWDL9jTIuE1tgRWX6GCQXuOWCKPG5grvc6CT2Osefj29SVIAfE7atPrD3QBGjNesyhBw0a97Y5YvVM2NV7zjDOoq5NiuseQsus8YzOLyN7d73Z3pWe3+xvGr/SWawWEyWmMqYEVG0Nl8u/TUeb8w3XeIQ==</D></RSAKeyValue>";
        public const string PublicKey = "<RSAKeyValue><Modulus>yAI5/1bP0BN4y0B/tqDGl15tVDA+9pT3sb666lV3cXrNJucupJHHrlP8rFRDJ9tc3uefPstKiBDGREF/oX153u+PizEZXm5mb8b1cmNbQj33yf6y7WtkyKIftDenf64OUpj92/U8YcFYFmmvHfBIfPlRvCXA8tBTbRcsDcgC7AIKpJMvR8wAqZYx5vkQ5WgsgA7hIQ1diEXuxk6lKKD/nWiEkozZvrvAo38/DTh0nUzdKhHA6nZTT+rFoKRDpLS6JxXIvSI+roUPb4WPv4gf8CYa5MNYLq/pxmacuoidqjCIjSUMmm31duITNNmtvyUkmv/i4ptp1Wqev+uNAZtSTw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private const string DataBase64Value = "tucrc/REfzqABO7dCl/JHxOifWgH6kcFoxf1y09lf4fK07cH0cbvnbFfkGg1On5xn85RY9IYKO5b30OLVFm86h3WcB8mM9DLw3gQjyOH7k7F9HK7rkqI8WcV6fKdpLtxk6MV78rVdzQD3XOIugJ8q6TR5mTqLxUxhU5C0LFGjCc7cC7NXITec8P/35yWIm/IrGpQjcMLI9vuHgxQ0NW1giERvJkSrVM28oSkxDPUvdH9HQB08zT6avvEhIk8Ntw94U1nMKHWR1iQ/ecVkRZJ55fXA5fJfWZFUW1pSwMgvGNnrBkLQ/rhSurWSRtyroChD2MUYOEbV0D1J9Zj7112+g==";
        [Fact]
        public void Test()
        {
            var data = RSA.Encrypt("ABC", PublicKey);
            Assert.Equal(DataBase64Value, data);
            Assert.Equal("ABC", RSA.Decrypt(data, Key));
        }
    }
}
