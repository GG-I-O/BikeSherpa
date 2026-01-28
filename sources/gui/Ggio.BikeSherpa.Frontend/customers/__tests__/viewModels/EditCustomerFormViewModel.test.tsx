import Customer from "@/customers/models/Customer";
import EditCustomerFormViewModel from "@/customers/viewModels/EditCustomerFormViewModel";
import { createRandomCustomer, linkType } from "@/fixtures/customer-fixtures";
import { ICustomerService } from "@/spi/CustomerSPI";
import { faker } from "@faker-js/faker";
import { mock } from "ts-jest-mocker";

const customerService = mock<ICustomerService>();

describe("NewCustomerFormViewModel", () => {
    const mockCustomer = createRandomCustomer(true, linkType.none);
    customerService.updateCustomer = jest.fn();
    const viewModel = new EditCustomerFormViewModel(customerService);

    beforeEach(() => {
        jest.clearAllMocks();
    })

    it("onSubmit calls updateCustomer with correct customer", () => {
        viewModel.onSubmit(mockCustomer);
        mockCustomer.address.name = mockCustomer.name;
        expect(customerService.updateCustomer).toHaveBeenCalledTimes(1);
        expect(customerService.updateCustomer).toHaveBeenCalledWith(mockCustomer);
    })

    describe("getEditCustomerSchema", () => {
        const existingCustomers: Customer[] = faker.helpers.multiple(() => createRandomCustomer(true, linkType.none), {
            count: 1
        });

        let customerToEdit: Customer;

        beforeEach(() => {
            customerToEdit = createRandomCustomer(true, linkType.none);
        })

        it("validates name is required", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.name = "";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Nom requis");
            }
        });

        it("validates code is required", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.code = "";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code requis");
            }
        });

        it("validates code max length", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.code = "ESTEST";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Code trop long");
            }
        });

        it("validates code uniqueness", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.code = existingCustomers[0].code;

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Le code doit être unique");
            }
        });

        it("validates email format", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);
            customerToEdit.email = "invalid.email";

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(false);
            if (!result.success) {
                expect(result.error.issues[0].message).toBe("Adresse e-mail non valide");
            }
        });

        it("accepts valid customer data", () => {
            //arrange
            const schema = viewModel.getEditCustomerSchema(customerToEdit, existingCustomers);

            //act
            const result = schema.safeParse(customerToEdit);

            //assert
            expect(result.success).toBe(true);
        });
    });
})