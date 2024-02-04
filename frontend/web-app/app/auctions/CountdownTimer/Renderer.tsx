import { zeroPad } from "react-countdown";

type Props = {
    days: number,
    hours: number,
    minutes: number,
    seconds: number,
    completed: boolean
};

export default function Renderer({ days, hours, minutes, seconds, completed }: Props) {
    const classes = getClasses(hours, days, completed);

    return (
        <div className={ classes }>
            { completed ? 
                <span>Auction finished</span> : 
                <span suppressHydrationWarning>
                    {zeroPad(days)}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}
                </span>
            }
        </div>
    )
};

function getClasses(hours: number, days: number, completed: boolean) {
    let classes = "flex justify-center border-2 border-white text-white py-1 px-2 rounded-lg";
        
    if(completed) classes += ' bg-red-600';

    classes += (days === 0 && hours < 10) ? 
        ' bg-amber-600' : ' bg-green-600';

    return classes;
}