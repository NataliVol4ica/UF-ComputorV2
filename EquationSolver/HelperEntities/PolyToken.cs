namespace PolynomialSolver.HelperEntities
{
    public struct PolyToken
    {
        public PolyToken(string str, TokenType tokenType)
        {
            this.str = str;
            this.tokenType = tokenType;
        }

        public TokenType tokenType;
        public string str;

        public override string ToString()
        {
            return str;
        }
    }
}