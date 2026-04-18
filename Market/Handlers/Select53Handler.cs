namespace tser
{
    internal class Select53Handler
    {
        private InputSimulator sim;
        private Mover mover;

        public Select53Handler(InputSimulator sim)
        {
            this.sim = sim;
            mover = new Mover(sim);
        }

        public async Task Run()
        {
            await mover.MoveAndClick(457, 393, 579, 403);

            await mover.MoveAndClick(457, 447, 574, 459);


            await mover.MoveAndClick(614, 392, 727, 405);

            await mover.MoveAndClick(615, 499, 729, 513);


            await mover.MoveAndClick(768, 394, 884, 405);
            await mover.MoveAndClick(764, 445, 880, 458);
        }
    }
}
