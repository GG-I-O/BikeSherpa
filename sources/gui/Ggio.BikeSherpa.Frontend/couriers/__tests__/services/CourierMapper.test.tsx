import { CourierDto } from "@/couriers/models/Courier";
import CourierMapper from "@/couriers/services/CourierMapper";
import { createRandomCourierDto } from "@/fixtures/courier-fixtures";

describe("CourierMapper", () => {
    it("CourierMapper converts a CourierDto into a Courier", () => {
        //arrange
        const courierDto: CourierDto = createRandomCourierDto();

        //act
        const courier = CourierMapper.CourierDtoToCourier(courierDto);

        //assert
        expect(courier.firstName).toBe(courierDto.data.firstName);
        expect(courier.lastName).toBe(courierDto.data.lastName);
        expect(courier.links).toStrictEqual([]);
        expect(courier.address).toStrictEqual(courierDto.data.address);
    })
})