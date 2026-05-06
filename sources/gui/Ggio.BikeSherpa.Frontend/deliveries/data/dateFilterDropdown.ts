import dateFilterEnum from "@/deliveries/data/dateFilterEnum";

const dateFilterDropdown =
    [
        {
            value: dateFilterEnum.Date,
            label: 'Date'
        },
        {
            value: dateFilterEnum.All,
            label: 'Toutes'
        }
    ];
export default dateFilterDropdown;