namespace tser
{
    internal class Select43Handler
    {
        private InputSimulator sim;
        private Mover mover;

        public Select43Handler(InputSimulator inputSimulator)
        {
            sim = inputSimulator;
            mover = new Mover(sim);
        }

        public async Task Run(HandlerContext context)
        {
            await mover.MoveAndClick(457, 393, 579, 403);

            await mover.MoveAndClick(459, 418, 576, 431);


            await mover.MoveAndClick(614, 392, 727, 405);

            await mover.MoveAndClick(615, 499, 711, 511);


            await mover.MoveAndClick(768, 394, 884, 405);
            await mover.MoveAndClick(764, 445, 880, 458);
        }
    }
}
