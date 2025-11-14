import { useForm } from "react-hook-form";

const EmployeeForm = ({ onSubmit, defaultValues }) => {
  const { register, handleSubmit } = useForm({ defaultValues });

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register("name")} placeholder="Employee Name" />
      <input {...register("email")} placeholder="Email" />
      <input {...register("position")} placeholder="Position" />
      <button type="submit">Save</button>
    </form>
  );
};

export default EmployeeForm;
