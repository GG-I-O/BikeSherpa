import Customer from "@/customers/models/Customer";
import NewCustomerFormViewModel from "@/customers/viewModels/NewCustomerFormViewModel";
import { createRandomCustomer, createRandomInputCustomer, linkType } from "@/fixtures/customer-fixtures";
import { ICustomerService } from "@/spi/CustomerSPI";
import { faker } from "@faker-js/faker";
import { mock } from "ts-jest-mocker";

const customerService = mock<ICustomerService>();

describe("NewCustomerFormViewModel", () => {
    const mockCustomer = createRandomInputCustomer();
    customerService.createCustomer = jest.fn();
    const viewModel = new NewCustomerFormViewModel(customerService);

    beforeEach(() => {
        jest.clearAllMocks();
    })

    it("onSubmit calls createCustomer with correct customer", () => {
        viewModel.onSubmit(mockCustomer);
        mockCustomer.address.name = mockCustomer.name;
        expect(customerService.createCustomer).toHaveBeenCalledTimes(1);
        expect(customerService.createCustomer).toHaveBeenCalledWith(mockCustomer);
    })

    it("onSubmit calls createCustomer with correct customer", () => {
        const mockResetCallback = jest.fn();
        viewModel.setResetCallback(mockResetCallback);
        viewModel.onSubmit(mockCustomer);

        expect(customerService.createCustomer).toHaveBeenCalledTimes(1);
        expect(mockResetCallback).toHaveBeenCalledTimes(1);
    })

    describe("getNewCustomerSchema", () => {
        const existingCustomers: Customer[] = faker.helpers.multiple(() => createRandomCustomer(true, linkType.none), {
            count: 1
        });


        let customerToValidate: Customer;

        beforeEach(() => {
            customerToValidate = createRandomCustomer(true, linkType.none);
        })

        it("validates name is required", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.name = "";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Nom requis");
            }
        });

        it("validates code is required", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.code = "";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code requis");
            }
        });

        it("validates code max length", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.code = "ESTEST";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code trop long");
            }
        });

        it("validates code uniqueness", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.code = existingCustomers[0].code;

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Le code doit être unique");
            }
        });

        it("validates email format", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);
            customerToValidate.email = "invalid.email";

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail invalide");
            }
        });

        it("accepts valid customer data", () => {
            //arrange
            const schema = viewModel.getNewCustomerSchema(existingCustomers);

            //act
            const result = schema.safeParse(customerToValidate);

            //assert
            expect(result.success).toBe(true);
        });
    });
})