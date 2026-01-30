import Courier from "@/couriers/models/Courier";
import EditCourierFormViewModel from "@/couriers/viewModels/EditCourierFormViewModel";
import { createRandomCourier, linkType } from "@/fixtures/courier-fixtures";
import { ICourierService } from "@/spi/CourierSPI";
import { faker } from "@faker-js/faker";
import { mock } from "ts-jest-mocker";

const courierService = mock<ICourierService>();

describe("NewCourierFormViewModel", () => {
    const mockCourier = createRandomCourier(true, linkType.none);
    courierService.updateCourier = jest.fn();
    const viewModel = new EditCourierFormViewModel(courierService);

    beforeEach(() => {
        jest.clearAllMocks();
    })

    it("onSubmit calls updateCourier with correct courier", () => {
        viewModel.onSubmit(mockCourier);
        mockCourier.address.name = `${mockCourier.firstName} ${mockCourier.lastName}`;
        expect(courierService.updateCourier).toHaveBeenCalledTimes(1);
        expect(courierService.updateCourier).toHaveBeenCalledWith(mockCourier);
    })

    describe("getEditCourierSchema", () => {
        const existingCouriers: Courier[] = faker.helpers.multiple(() => createRandomCourier(true, linkType.none), {
            count: 1
        });

        let courierToEdit: Courier;

        beforeEach(() => {
            courierToEdit = createRandomCourier(true, linkType.none);
        })

        it("validates firstName is required", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.firstName = "";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Prénom requis");
            }
        });

        it("validates lastName is required", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.lastName = "";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Nom requis");
            }
        });

        it("validates code is required", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.code = "";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code requis");
            }
        });

        it("validates phone is required", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.phoneNumber = "";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Numéro de téléphone requis");
            }
        });

        it("validates email is required", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.email = "";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail requise");
            }
        });

        it("validates code max length", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.code = "ESTEST";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code trop long");
            }
        });

        it("validates code uniqueness", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.code = existingCouriers[0].code;

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Le code doit être unique");
            }
        });

        it("validates email format", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.email = "invalid.email";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail invalide");
            }
        });

        it("validates phone format", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);
            courierToEdit.phoneNumber = "invalid.phone.number";

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Numéro de téléphone invalide");
            }
        });

        it("accepts valid courier data", () => {
            //arrange
            const schema = viewModel.getEditCourierSchema(courierToEdit, existingCouriers);

            //act
            const result = schema.safeParse(courierToEdit);

            //assert
            expect(result.success).toBe(true);
        });
    });
})